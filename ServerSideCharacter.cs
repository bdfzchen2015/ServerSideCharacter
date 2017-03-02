using System;
using System.Collections.Concurrent;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using System.Xml;
using Terraria.IO;
using Terraria.Localization;
using ServerSideCharacter.XMLHelper;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using ServerSideCharacter.ServerCommand;

namespace ServerSideCharacter
{
	public class ServerSideCharacter : Mod
	{
		public static ServerSideCharacter instance;

		public static XMLData xmlData;

		public static XMLWriter MainWriter;

		public static Thread CheckDisconnect;

		public static string Version = "V1.0b";

		public static List<Command> Commands = new List<Command>();

		private static ConcurrentDictionary<int, SaveInfo> PlayerActiveTable = new ConcurrentDictionary<int, SaveInfo>();

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
				//如果数据中没有玩家的信息
				if (!xmlData.Data.ContainsKey(Main.player[playerNumber].name))
				{
					try
					{
						//创建新的玩家数据
						ServerPlayer serverPlayer = ServerPlayer.CreateNewPlayer(Main.player[playerNumber]);
						serverPlayer.prototypePlayer = Main.player[playerNumber];
						xmlData.Data.Add(Main.player[playerNumber].name, serverPlayer);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
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
				player.prototypePlayer = Main.player[plr];

				//增加限制性debuff
				player.ApplyLockBuffs();

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
				NetSync.SyncPlayerBanks(plr, -1, -1);
				PlayerHooks.SyncPlayer(Main.player[plr], toWho, fromWho, false);
				PlayerActiveTable[plr] = new SaveInfo() { Name = Main.player[plr].name, Active = true };
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
				Console.WriteLine("[ServerSideCharacter Mod, Author: DXTsT	Version: " + Version + "]");
				//if (!Directory.Exists("SSC"))
				//{
				//	Directory.CreateDirectory("SSC");
				//	string save = Path.Combine("SSC", "datas.xml");
				//	XMLWriter writer = new XMLWriter(save);
				//	writer.Create();
				//	ServerPlayer newPlayer = ServerPlayer.CreateNewPlayer("DXTsT");
				//	writer.Write(newPlayer);
				//	MainWriter = writer;
				//	Console.WriteLine("Saved data: " + save);
				//}
				xmlData = new XMLData("SSC/datas.xml");
				Console.WriteLine("Data loaded!");

				CheckDisconnect = new Thread(() =>
				{
					while (!Netplay.disconnect)
					{

						Thread.Sleep(100);
					}
					lock (ServerSideCharacter.xmlData.Data)
					{
						foreach (var player in ServerSideCharacter.xmlData.Data)
						{
							try
							{
								ServerSideCharacter.MainWriter.SavePlayer(player.Value);
							}
							catch (Exception ex)
							{
								Console.WriteLine(ex);
							}
							Console.WriteLine("\nOn Server Close: Saved " + player.Key);
						}
					}

				});
				CheckDisconnect.Start();
			}
			else
			{
				CommandDelegate.SetUpCommands(Commands);
			}
		}

		private void Netplay_OnDisconnect()
		{
			NetSync.SendRequestSave(Main.myPlayer);
			Main.NewText("Saving");
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
			else if (msgType == SSCMessageType.SyncPlayerBank)
			{
				int id = reader.ReadByte();
				if (id == Main.myPlayer && !Main.ServerSideCharacter && !Main.player[id].IsStackingItems())
				{
					return;
				}
				Player player = Main.player[id];
				lock (player)
				{
					for (int k = 0; k < player.bank.item.Length; k++)
					{
						int type = reader.ReadInt32();
						int prefix = reader.ReadInt16();
						int stack = reader.ReadInt16();
						player.bank.item[k].SetDefaults(type);
						player.bank.item[k].Prefix(prefix);
						player.bank.item[k].stack = stack;
					}
					for (int k = 0; k < player.bank2.item.Length; k++)
					{
						int type = reader.ReadInt32();
						int prefix = reader.ReadInt16();
						int stack = reader.ReadInt16();
						player.bank2.item[k].SetDefaults(type);
						player.bank2.item[k].Prefix(prefix);
						player.bank2.item[k].stack = stack;
					}
					for (int k = 0; k < player.bank3.item.Length; k++)
					{
						int type = reader.ReadInt32();
						int prefix = reader.ReadInt16();
						int stack = reader.ReadInt16();
						player.bank3.item[k].SetDefaults(type);
						player.bank3.item[k].Prefix(prefix);
						player.bank3.item[k].stack = stack;
					}
				}
			}
			else if (msgType == SSCMessageType.RequestSaveData)
			{
				int plr = reader.ReadByte();
				Player p = Main.player[plr];
				ServerPlayer player = xmlData.Data[p.name];
				player.CopyFrom(Main.player[plr]);
				try
				{
					MainWriter.SavePlayer(player);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
				Console.WriteLine("Saved " + player.Name);
			}
			else if (msgType == SSCMessageType.RequestRegister)
			{
				int plr = reader.ReadByte();
				string password = reader.ReadString();
				Player p = Main.player[plr];
				ServerPlayer player = xmlData.Data[p.name];
				if (player.HasPassword)
				{
					NetMessage.SendData(MessageID.ChatText, plr, -1, "You cannot register twice!",
						255, 255, 0, 0);
					return;

				}
				else
				{
					player.HasPassword = true;
					player.Password = password;
					NetMessage.SendData(MessageID.ChatText, plr, -1,
						string.Format("You have successfully set your password as {0}. Remember it!", password),
						255, 50, 255, 50);
				}
			}
			else if (msgType == SSCMessageType.SendLoginPassword)
			{
				int plr = reader.ReadByte();
				string password = reader.ReadString();
				Player p = Main.player[plr];
				ServerPlayer player = xmlData.Data[p.name];
				
				if (!player.HasPassword)
				{
					NetMessage.SendData(MessageID.ChatText, plr, -1, "You should first register an account use /register <password> !",
						255, 255, 0, 0);
					return;

				}
				else
				{
					bool isPasswordCorrrect = password.Equals(player.Password);
					if (isPasswordCorrrect)
					{
						player.IsLogin = true;
						NetMessage.SendData(MessageID.ChatText, plr, -1,
							string.Format("You have successfully logged in as {0}", player.Name),
							255, 50, 255, 50);
						NetMessage.SendData(MessageID.ChatText, plr, -1,
							"Please wait for a few seconds and then you can move",
							255, 255, 255, 0);
					}
					else
					{
						NetMessage.SendData(MessageID.ChatText, plr, -1,
							"Wrong password! Please try again!",
							255, 255, 20, 0);
					}
				}
			}
			else
			{
				Console.WriteLine("Unexpected message type!");
			}
		}


		public override void ChatInput(string text, ref bool broadcast)
		{
			if (text[0] == '/')
			{
				if (Main.netMode != 0)
				{
					text = text.Substring(1);
					int index = text.IndexOf(' ');
					string command;
					string[] args;
					if (index < 0)
					{
						command = text;
						args = new string[0];
					}
					else
					{
						command = text.Substring(0, index);
						args = text.Substring(index + 1).Split(' ');
					}
					broadcast = false;
					int cmdIndex = Commands.FindIndex(cmd => cmd.Name == command);
					Main.NewText(cmdIndex.ToString());
					foreach (var cmd in Commands)
					{
						Main.NewText(cmd.Name);
					}
					if (cmdIndex != -1)
					{
						Command cmd = Commands[cmdIndex];
						cmd.CommandAction(args);
					}
				}
				//if (command == "save")
				//{
				//	NetSync.SendRequestSave(Main.myPlayer);
				//	Main.NewText("Saving");
				//}
				//if (command == "register")
				//{
				//	try
				//	{
				//		NetSync.SendSetPassword(Main.myPlayer, args[0]);
				//	}
				//	catch
				//	{
				//		Main.NewText("Invalid Sytanx! Usage: /register <your password>", 255, 255, 0);
				//	}
				//}
				//if(command == "login")
				//{
				//	try
				//	{
				//		NetSync.SendLoginPassword(Main.myPlayer, args[0]);
				//	}
				//	catch
				//	{
				//		Main.NewText("Invalid Sytanx! Usage: /login <your password>", 255, 255, 0);
				//	}
				//}
			}
		}
	}
}