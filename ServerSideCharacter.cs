using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using System.Xml;
using Terraria.IO;
using Terraria.Localization;
using ServerSideCharacter.XMLHelper;
using System.Text;

namespace ServerSideCharacter
{
	public class ServerSideCharacter : Mod
	{
		public static ServerSideCharacter instance;

		public static PlayerDatas xmlData;

		public ServerSideCharacter()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadSounds = true,
				AutoloadGores = true
			};
		}
		public override bool HijackGetData(ref byte messageType, ref BinaryReader reader, int playerNumber)
		{
			if (messageType == MessageID.SpawnPlayer)
			{
				int id = reader.ReadByte();
				if (Main.netMode == 2)
				{
					id = playerNumber;
				}
				Player player = Main.player[id];
				player.SpawnX = reader.ReadInt16();
				player.SpawnY = reader.ReadInt16();
				player.Spawn();
				if (id == Main.myPlayer && Main.netMode != 2)
				{
					Main.ActivePlayerFileData.StartPlayTimer();
					Player.Hooks.EnterWorld(Main.myPlayer);
				}
				if (Main.netMode != 2 || Netplay.Clients[playerNumber].State < 3)
				{
					return true;
				}
				if (Netplay.Clients[playerNumber].State == 3)
				{
					Netplay.Clients[playerNumber].State = 10;
					NetMessage.greetPlayer(playerNumber);
					NetMessage.buffer[playerNumber].broadcast = true;
					SyncConnectedPlayer(playerNumber);
					NetMessage.SendData(MessageID.SpawnPlayer, -1, playerNumber, "", playerNumber, 0f, 0f, 0f, 0, 0, 0);
					NetMessage.SendData(MessageID.AnglerQuest, playerNumber, -1, Main.player[playerNumber].name, Main.anglerQuest, 0f, 0f, 0f, 0, 0, 0);
					return true;
				}
				NetMessage.SendData(MessageID.SpawnPlayer, -1, playerNumber, "", playerNumber, 0f, 0f, 0f, 0, 0, 0);
				return true;
			}
			return false;
		}

		public static void SyncConnectedPlayer(int plr)
		{
			SyncOnePlayer(plr, -1, plr);
			for (int i = 0; i < 255; i++)
			{
				if (plr != i && Main.player[i].active)
				{
					SyncOnePlayer(i, plr, -1);
				}
			}
			SendNPCHousesAndTravelShop(plr);
			SendAnglerQuest(plr);
			EnsureLocalPlayerIsPresent();
		}
		private static void SyncOnePlayer(int plr, int toWho, int fromWho)
		{
			int num = 0;
			if (Main.player[plr].active)
			{
				num = 1;
			}
			if (Netplay.Clients[plr].State == 10)
			{
				NetMessage.SendData(MessageID.PlayerActive, toWho, fromWho, "", plr, num, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(MessageID.SyncPlayer, toWho, fromWho, Main.player[plr].name, plr, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(MessageID.PlayerControls, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				//NetMessage.SendData(MessageID.PlayerHealth, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				NetSync.SyncPlayerHealth(plr, -1, -1);
				NetMessage.SendData(MessageID.PlayerPvP, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(MessageID.PlayerTeam, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				//NetMessage.SendData(MessageID.PlayerMana, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				NetSync.SyncPlayerMana(plr, -1, -1);
				NetMessage.SendData(MessageID.PlayerBuffs, toWho, fromWho, "", plr, 0f, 0f, 0f, 0, 0, 0);
				string name = Main.player[plr].name;
				ServerPlayer player = xmlData.Data[name];
				player.inventroy.CopyTo(Main.player[plr].inventory, 0);
				player.armor.CopyTo(Main.player[plr].armor, 0);
				player.dye.CopyTo(Main.player[plr].dye, 0);
				player.miscEquips.CopyTo(Main.player[plr].miscEquips, 0);
				player.miscDye.CopyTo(Main.player[plr].miscDyes, 0);
				Main.player[plr].trashItem = new Item();
				foreach (var i in Main.player[plr].armor)
				{
					Console.Write(i.type + " ");
				}
				for (int i = 0; i < 59; i++)
				{
					NetMessage.SendData(MessageID.SyncEquipment, -1, -1, Main.player[plr].inventory[i].name, plr, i, Main.player[plr].inventory[i].prefix, 0f, 0, 0, 0);
				}
				for (int j = 0; j < Main.player[plr].armor.Length; j++)
				{
					NetMessage.SendData(MessageID.SyncEquipment, -1, -1, Main.player[plr].armor[j].name, plr, (59 + j), Main.player[plr].armor[j].prefix, 0f, 0, 0, 0);
				}
				for (int k = 0; k < Main.player[plr].dye.Length; k++)
				{
					NetMessage.SendData(MessageID.SyncEquipment, -1, -1, Main.player[plr].dye[k].name, plr, (58 + Main.player[plr].armor.Length + 1 + k), Main.player[plr].dye[k].prefix, 0f, 0, 0, 0);
				}
				for (int l = 0; l < Main.player[plr].miscEquips.Length; l++)
				{
					NetMessage.SendData(MessageID.SyncEquipment, -1, -1, "", plr, 58 + Main.player[plr].armor.Length + Main.player[plr].dye.Length + 1 + l, Main.player[plr].miscEquips[l].prefix, 0f, 0, 0, 0);
				}
				for (int m = 0; m < Main.player[plr].miscDyes.Length; m++)
				{
					NetMessage.SendData(MessageID.SyncEquipment, -1, -1, "", plr, 58 + Main.player[plr].armor.Length + Main.player[plr].dye.Length + Main.player[plr].miscEquips.Length + 1 + m, Main.player[plr].miscDyes[m].prefix, 0f, 0, 0, 0);
				}
				PlayerHooks.SyncPlayer(Main.player[plr], toWho, fromWho, false);
				if (!Netplay.Clients[plr].IsAnnouncementCompleted)
				{
					Netplay.Clients[plr].IsAnnouncementCompleted = true;
					NetMessage.SendData(MessageID.ChatText, -1, plr, Main.player[plr].name + " joined the Game. Welcome!", 255, 255f, 240f, 20f, 0, 0, 0);
					if (Main.dedServ)
					{
						Console.WriteLine(Main.player[plr].name + " joined the Game. Welcome!");

					}
					return;
				}
			}
			else
			{
				num = 0;
				NetMessage.SendData(MessageID.PlayerActive, -1, plr, "", plr, num, 0f, 0f, 0, 0, 0);
				if (Netplay.Clients[plr].IsAnnouncementCompleted)
				{
					Netplay.Clients[plr].IsAnnouncementCompleted = false;
					NetMessage.SendData(MessageID.ChatText, -1, plr, Netplay.Clients[plr].Name + " lefted the Game!", 255, 255f, 240f, 20f, 0, 0, 0);
					if (Main.dedServ)
					{
						Console.WriteLine(Netplay.Clients[plr].Name + " lefted the Game!");
					}
					Netplay.Clients[plr].Name = "Anonymous";
				}
			}
		}

		private static void SendNPCHousesAndTravelShop(int plr)
		{
			bool flag = false;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && Main.npc[i].townNPC && NPC.TypeToHeadIndex(Main.npc[i].type) != -1)
				{
					if (!flag && Main.npc[i].type == 368)
					{
						flag = true;
					}
					int num = 0;
					if (Main.npc[i].homeless)
					{
						num = 1;
					}
					NetMessage.SendData(MessageID.NPCHome, plr, -1, "", i, (float)Main.npc[i].homeTileX, (float)Main.npc[i].homeTileY, (float)num, 0, 0, 0);
				}
			}
			if (flag)
			{
				NetMessage.SendTravelShop(plr);
			}
		}

		public static void SendAnglerQuest(int remoteClient)
		{
			if (Main.netMode != 2)
			{
				return;
			}
			if (remoteClient == -1)
			{
				for (int i = 0; i < 255; i++)
				{
					if (Netplay.Clients[i].State == 10)
					{
						NetMessage.SendData(MessageID.AnglerQuest, i, -1, Main.player[i].name, Main.anglerQuest, 0f, 0f, 0f, 0, 0, 0);
					}
				}
				return;
			}
			if (Netplay.Clients[remoteClient].State == 10)
			{
				NetMessage.SendData(MessageID.AnglerQuest, remoteClient, -1, Main.player[remoteClient].name, Main.anglerQuest, 0f, 0f, 0f, 0, 0, 0);
			}
		}

		private static void EnsureLocalPlayerIsPresent()
		{
			if (!Main.autoShutdown)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < 255; i++)
			{
				if (Netplay.Clients[i].State == 10 && Netplay.Clients[i].Socket.GetRemoteAddress().IsLocalHost())
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Console.WriteLine(Language.GetTextValue("Net.ServerAutoShutdown"));
				WorldFile.saveWorld();
				Netplay.disconnect = true;
			}
		}

		public override void Load()
		{
			instance = this;
			if (Main.dedServ)
			{
				Main.ServerSideCharacter = true;
				if (!Directory.Exists("SSC"))
				{
					Directory.CreateDirectory("SSC");
					XmlDocument xmlDoc = new XmlDocument();
					XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
					xmlDoc.AppendChild(node);
					//创建根节点    
					XmlNode root = xmlDoc.CreateElement("Players");
					xmlDoc.AppendChild(root);
					XmlNode playerNode = xmlDoc.CreateNode(XmlNodeType.Element, "Player", null);
					NodeHelper.CreateNode(xmlDoc, playerNode, "name", "DXTsT");
					NodeHelper.CreateNode(xmlDoc, playerNode, "hash", ServerPlayer.GenHashCode("DXTsT"));
					NodeHelper.CreateNode(xmlDoc, playerNode, "password", "12345");
					NodeHelper.CreateNode(xmlDoc, playerNode, "lifeMax", "100");
					NodeHelper.CreateNode(xmlDoc, playerNode, "statlife", "100");
					NodeHelper.CreateNode(xmlDoc, playerNode, "manaMax", "20");
					NodeHelper.CreateNode(xmlDoc, playerNode, "statmana", "20");
					for (int i = 0; i < 59; i++)
					{
						var node1 = (XmlElement)NodeHelper.CreateNode(xmlDoc, playerNode, "slot_" + i, "0");
						node1.SetAttribute("prefix", "0");
						node1.SetAttribute("stack", "0");
					}
					for (int i = 59; i < 79; i++)
					{
						var node1 = (XmlElement)NodeHelper.CreateNode(xmlDoc, playerNode, "slot_" + i, "0");
						node1.SetAttribute("prefix", "0");
					}
					for (int i = 79; i < 89; i++)
					{
						NodeHelper.CreateNode(xmlDoc, playerNode, "slot_" + i, "0");
					}
					for (int i = 89; i < 94; i++)
					{
						NodeHelper.CreateNode(xmlDoc, playerNode, "slot_" + i, "0");
					}
					for (int i = 94; i < 99; i++)
					{
						NodeHelper.CreateNode(xmlDoc, playerNode, "slot_" + i, "0");
					}
					root.AppendChild(playerNode);
					string save = Path.Combine("SSC", "datas.xml");

					using (XmlTextWriter xtw = new XmlTextWriter(save, Encoding.UTF8))
					{
						xtw.Formatting = Formatting.None;
						xmlDoc.Save(xtw);
					}
					Console.WriteLine("Saved data: " + save);
				}
				else
				{
					xmlData = new PlayerDatas("SSC/datas.xml");
				}
				Console.WriteLine("Data loaded!");

			}
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			SSCMessageType msgType = (SSCMessageType)reader.ReadInt32();
			if (msgType == SSCMessageType.SyncPlayerHealth)
			{
				int id = reader.ReadByte();

				if (id == Main.myPlayer && !Main.ServerSideCharacter)
				{
					return;
				}
				Player player = Main.player[id];
				player.statLife = reader.ReadInt32();
				player.statLifeMax = reader.ReadInt32();
				if (player.statLifeMax < 100)
				{
					player.statLifeMax = 100;
				}
				player.dead = player.statLife <= 0;
			}
			else if (msgType == SSCMessageType.SyncPlayerMana)
			{
				int id = reader.ReadByte();
				if (Main.myPlayer == id && !Main.ServerSideCharacter)
				{
					return;
				}
				int statMana = reader.ReadInt32();
				int statManaMax = reader.ReadInt32();
				Main.player[id].statMana = statMana;
				Main.player[id].statManaMax = statManaMax;
			}
		}
	}
}