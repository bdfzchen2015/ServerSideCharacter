#define DEBUGMODE
using System;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using System.Xml;
using Terraria.IO;
using Terraria.Localization;
using Terraria.DataStructures;
using ServerSideCharacter.XMLHelper;
using System.Text;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using ServerSideCharacter.ServerCommand;
using ServerSideCharacter.Plugin;
using ServerSideCharacter.GroupManage;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Chat;

namespace ServerSideCharacter
{
	public class ServerSideCharacter : Mod
	{
		public static ServerSideCharacter instance;

		public static XMLData xmlData;

		public static XMLWriter MainWriter;

		public static Thread CheckDisconnect;

		public static string Version = "V1.0.2";

		public static List<Command> Commands = new List<Command>();

		private static ConcurrentDictionary<int, SaveInfo> PlayerActiveTable = new ConcurrentDictionary<int, SaveInfo>();

		public static TextLog Logger;

		public static string AuthCode = "";

		public static Point TilePos1 = new Point();

		public static Point TilePos2 = new Point();

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
			else if (messageType == MessageID.ChatText)
			{
				int playerID = reader.ReadByte();
				if (Main.netMode == 2)
				{
					playerID = playerNumber;
				}
				Color c = reader.ReadRGB();
				if (Main.netMode == 2)
				{
					c = new Color(255, 255, 255);
				}
				string text = reader.ReadString();
				if (Main.netMode == 1)
				{
					string text2 = text.Substring(text.IndexOf('>') + 1);
					if (playerID < 255)
					{
						Main.player[playerID].chatOverhead.NewMessage(text2, Main.chatLength / 2);
					}
					Main.NewTextMultiline(text, false, c, -1);
				}
				else
				{
					Player p = Main.player[playerID];
					ServerPlayer player = p.GetServerPlayer();
					string prefix = "";
					if (player.PermissionGroup.GroupName == "spadmin")
					{
						prefix = "[SuperAdmin] ";
						c = Color.Red;
					}
					NetMessage.SendData(25, -1, -1, prefix + "<" + p.name + "> " + text, playerID, (float)c.R, (float)c.G, (float)c.B, 0, 0, 0);
					if (Main.dedServ)
					{

						Console.WriteLine(string.Format("{0}<" + Main.player[playerID].name + "> " + text, prefix));
					}
				}
				return true;
			}
			else if(messageType == MessageID.TileChange)
			{
				if (Main.netMode == 2)
				{
					Player p = Main.player[playerNumber];
					ServerPlayer player = p.GetServerPlayer();
					int action = reader.ReadByte();
					short X = reader.ReadInt16();
					short Y = reader.ReadInt16();
					short type = reader.ReadInt16();
					int style = reader.ReadByte();
					if (CheckSpawn(X, Y))
					{
						NetMessage.SendData(MessageID.ChatText, playerNumber, -1, "Warning: You do not have the permission to change this tile", playerNumber, 255, 20, 20, 0, 0, 0);
						NetMessage.SendTileSquare(-1, X, Y, 4);
						return true;
					}
				}
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
				if (toWho == -1)
				{
					player.IsLogin = false;
					//增加限制性debuff
					player.ApplyLockBuffs();
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

		public override void PostSetupContent()
		{
			if (Main.dedServ)
			{
				SetupDefaults();
				//尝试在tml做插件，但是失败了QaQ
				//等待你们来修复 /(ㄒoㄒ)/~~
				//PluginLoader.LoadPlugins();


				if (!Directory.Exists("SSC"))
				{
					Directory.CreateDirectory("SSC");
					string save = Path.Combine("SSC", "datas.xml");
					XMLWriter writer = new XMLWriter(save);
					writer.Create();
					Player tmp = new Player();
					tmp.name = "DXTsT";
					ServerPlayer newPlayer = ServerPlayer.CreateNewPlayer(tmp);
					writer.Write(newPlayer);
					MainWriter = writer;
					Console.WriteLine("Saved data: " + save);
				}

				if (!System.IO.File.Exists("SSC/authcode"))
				{
					string authcode = Convert.ToString((Main.rand.Next(300000) + DateTime.Now.Millisecond) % 65536 + 65535, 16);
					AuthCode = authcode;
					using (StreamWriter sw = new StreamWriter("SSC/authcode"))
					{
						sw.WriteLine(authcode);
					}
				}
				else
				{
					using(StreamReader sr = new StreamReader("SSC/authcode"))
					{
						AuthCode = sr.ReadLine();
					}
				}

				

				xmlData = new XMLData("SSC/datas.xml");
				Logger = new TextLog("ServerLog.txt", false);
				CommandBoardcast.ShowMessage("Data loaded!");
				CommandBoardcast.ShowMessage("You can type /auth " + AuthCode + " to become super admin");

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
							CommandBoardcast.ShowMessage("\nOn Server Close: Saved " + player.Key);
						}
					}
					lock (ServerSideCharacter.Logger)
					{
						Logger.Dispose();
					}

				});
				CheckDisconnect.Start();
			}
		}

		public override void Load()
		{
			instance = this;
			if (Main.dedServ)
			{
				Main.ServerSideCharacter = true;
				Console.WriteLine("[ServerSideCharacter Mod, Author: DXTsT	Version: " + Version + "]");
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
			try
			{
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

#if DEBUGMODE
					CommandBoardcast.ShowSavePlayer(player);
#endif
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
				else if (msgType == SSCMessageType.KillCommand)
				{
					int plr = reader.ReadByte();
					int target = reader.ReadByte();
					Player p = Main.player[plr];
					ServerPlayer player = xmlData.Data[p.name];
					if (!player.IsLogin) return;
					if (player.PermissionGroup.HasPermission("kill"))
					{
						Main.player[target].KillMe(PlayerDeathReason.ByCustomReason(" was killed by absolute power!"),
							23333, 0, false);
						NetMessage.SendPlayerDeath(target, PlayerDeathReason.ByCustomReason(" was killed by absolute power!"),
							23333, 0, false, -1, -1);
					}
					else
					{
						NetMessage.SendData(MessageID.ChatText, plr, -1,
								"You don't have the permission to this command.",
								255, 255, 20, 0);
					}
				}
				else if (msgType == SSCMessageType.ListCommand)
				{
					int plr = reader.ReadByte();
					Player p = Main.player[plr];
					ServerPlayer player = xmlData.Data[p.name];
					if (!player.IsLogin) return;
					if (player.PermissionGroup.HasPermission("ls"))
					{
						try
						{
							StringBuilder sb = new StringBuilder();
							sb.AppendLine("Player ID    Name    Hash    Permission Group    LifeMax");
							foreach (var pla in xmlData.Data)
							{
								Player player1 = pla.Value.prototypePlayer;
								string line = string.Concat(
									player1 != null && player1.active ? player1.whoAmI.ToString() : "N/A",
									"    ",
									pla.Value.Name,
									"    ",
									pla.Value.Hash,
									"    ",
									pla.Value.PermissionGroup.GroupName,
									"    ",
									pla.Value.LifeMax,
									"    "
									);
								sb.AppendLine(line);
							}
							NetMessage.SendData(MessageID.ChatText, plr, -1,
									sb.ToString() + "\n" + xmlData.Data.Count,
									255, 255, 255, 0);
						}
						catch (Exception ex)
						{
							CommandBoardcast.ShowError(ex);
						}
					}
					else
					{
						NetMessage.SendData(MessageID.ChatText, plr, -1,
								"You don't have the permission to this command.",
								255, 255, 20, 0);
					}
				}
				else if (msgType == SSCMessageType.RequestSetGroup)
				{
					int plr = reader.ReadByte();
					string hash = reader.ReadString();
					string group = reader.ReadString();
					Player p = Main.player[plr];
					ServerPlayer player = xmlData.Data[p.name];
					if (!player.IsLogin) return;
					if (player.PermissionGroup.HasPermission("group"))
					{
						try
						{
							ServerPlayer targetPlayer = FindPlayer(hash);
							targetPlayer.PermissionGroup = GroupType.Groups[group];
							NetMessage.SendData(MessageID.ChatText, plr, -1,
								string.Format("Successfully set {0} to group '{1}'", targetPlayer.Name, group),
								255, 50, 255, 50);
						}
						catch
						{
							NetMessage.SendData(MessageID.ChatText, plr, -1,
								"Cannot find this player or group name invalid!",
								255, 255, 20, 0);
							return;
						}
					}
					else
					{
						NetMessage.SendData(MessageID.ChatText, plr, -1,
								"You don't have the permission to this command.",
								255, 255, 20, 0);
					}
				}
				else if (msgType == SSCMessageType.LockPlayer)
				{
					int plr = reader.ReadByte();
					int target = reader.ReadByte();
					int time = reader.ReadInt32();
					Player p = Main.player[plr];
					Player target0 = Main.player[target];
					ServerPlayer target1 = xmlData.Data[target0.name];
					ServerPlayer player = xmlData.Data[p.name];
					if (!player.IsLogin) return;
					if (player.PermissionGroup.HasPermission("lock"))
					{
						target1.ApplyLockBuffs(time);
						NetMessage.SendData(MessageID.ChatText, plr, -1,
								string.Format("You have successfully locked {0} for {1} frames", target1.Name, time),
								255, 50, 255, 50);
					}
					else
					{
						NetMessage.SendData(MessageID.ChatText, plr, -1,
								"You don't have the permission to this command.",
								255, 255, 20, 0);
					}
				}
				else if (msgType == SSCMessageType.ButcherCommand)
				{
					int plr = reader.ReadByte();
					Player p = Main.player[plr];
					ServerPlayer player = xmlData.Data[p.name];
					if (!player.IsLogin) return;
					if (player.PermissionGroup.HasPermission("butcher"))
					{
						int kills = 0;
						for (int i = 0; i < Main.npc.Length; i++)
						{
							if (Main.npc[i].active && (!Main.npc[i].townNPC && Main.npc[i].netID != NPCID.TargetDummy))
							{
								Main.npc[i].StrikeNPC(100000000, 0, 0);
								NetMessage.SendData((int)MessageID.StrikeNPC, -1, -1, "", i, 100000000, 0, 0);
								kills++;
							}
						}
						CommandBoardcast.SendInfoToAll(string.Format("{0} butchered {1} NPCs.", player.Name, kills));
					}
					else
					{
						CommandBoardcast.SendErrorToPlayer(plr, "You don't have the permission to this command.");
					}
				}
				else if (msgType == SSCMessageType.TPCommand)
				{
					int plr = reader.ReadByte();
					int target = reader.ReadByte();
					Player p = Main.player[plr];
					ServerPlayer player = xmlData.Data[p.name];
					ServerPlayer targetPlayer = xmlData.Data[Main.player[target].name];
					if (!player.IsLogin || target == plr) return;
					if (player.PermissionGroup.HasPermission("tp"))
					{
						if (targetPlayer.prototypePlayer != null && targetPlayer.prototypePlayer.active)
						{
							p.Teleport(Main.player[target].position);
							NetSync.SendTeleport(plr, Main.player[target].position);
							CommandBoardcast.SendInfoToPlayer(plr, "You have teleproted to " + targetPlayer.Name);
							CommandBoardcast.SendInfoToPlayer(target, player.Name + " has teleproted to you!");
						}
						else
						{
							CommandBoardcast.SendErrorToPlayer(plr, "Cannot find this player");
						}
					}
					else
					{
						CommandBoardcast.SendErrorToPlayer(plr, "You don't have the permission to this command.");
					}
				}
				else if (msgType == SSCMessageType.TimeCommand)
				{
					int plr = reader.ReadByte();
					bool set = reader.ReadBoolean();
					int time = reader.ReadInt32();
					bool day = reader.ReadBoolean();
					Player p = Main.player[plr];
					ServerPlayer player = xmlData.Data[p.name];
					if (!player.IsLogin) return;
					if (player.PermissionGroup.HasPermission("time"))
					{
						if (!set)
						{
							double time1 = GetTime();
							CommandBoardcast.SendInfoToPlayer(plr, string.Format("The current time is {0}:{1:D2}.", 
								(int)Math.Floor(time1), (int)Math.Round((time1 % 1.0) * 60.0)));
						}
						else
						{
							Main.time = time;
							Main.dayTime = day;
							NetSync.SendTimeSet(Main.time, Main.dayTime);
							double time1 = GetTime();
							CommandBoardcast.SendInfoToAll(string.Format("{0} set the time to {1}:{2:D2}.", player.Name,
								(int)Math.Floor(time1), (int)Math.Round((time1 % 1.0) * 60.0)));
						}
					}
					else
					{
						CommandBoardcast.SendErrorToPlayer(plr, "You don't have the permission to this command.");
					}

				}
				else if(msgType == SSCMessageType.SendTimeSet)
				{
					double time = reader.ReadDouble();
					bool day = reader.ReadBoolean();
					short sunY = reader.ReadInt16();
					short moonY = reader.ReadInt16();
					if(Main.netMode == 1)
					{
						Main.time = time;
						Main.dayTime = day;
						Main.sunModY = sunY;
						Main.moonModY = moonY;
					}
				}
				else if(msgType == SSCMessageType.HelpCommand)
				{
					int plr = reader.ReadByte();
					StringBuilder sb = new StringBuilder();
					sb.Append("Current commands:\n");
					Player p = Main.player[plr];
					ServerPlayer player = xmlData.Data[p.name];
					int i = 0;

					foreach(var command in Commands)
					{
						if (player.PermissionGroup.HasPermission(command.Name))
						{
							sb.Append("/" + command.Name + "  ");
							i++;
							if(i > 4)
							{
								i = 0;
								sb.Append("\n");
							}
						}
					}
					CommandBoardcast.SendInfoToPlayer(plr, sb.ToString());
				}
				else if(msgType == SSCMessageType.RequestItem)
				{
					int plr = reader.ReadByte();
					int type = reader.ReadInt32();
					Player p = Main.player[plr];
					ServerPlayer player = xmlData.Data[p.name];
					if (!player.IsLogin) return;
					if (player.PermissionGroup.HasPermission("item"))
					{
						Item item = new Item();
						item.netDefaults(type);
						Item.NewItem(p.position, Vector2.Zero, type, item.maxStack);
						CommandBoardcast.SendInfoToPlayer(plr, string.Format("Sever has give you {0} {1}", item.maxStack, Main.itemName[type]));
					}
					else
					{
						CommandBoardcast.SendErrorToPlayer(plr, "You don't have the permission to this command.");
					}
				}
				else if (msgType == SSCMessageType.TeleportPalyer)
				{

					Vector2 dest = reader.ReadVector2();
					if (Main.netMode == 1)
					{
						Main.LocalPlayer.Teleport(dest);
					}
				}
				else if(msgType == SSCMessageType.RequestAuth)
				{
					int plr = reader.ReadByte();
					string code = reader.ReadString();
					Player p = Main.player[plr];
					CommandBoardcast.ShowMessage(p.name + " has tried to auth with code " + code);
					if(code.Equals(AuthCode))
					{
						ServerPlayer targetPlayer = p.GetServerPlayer();
						targetPlayer.PermissionGroup = GroupType.Groups["spadmin"];
						CommandBoardcast.SendInfoToPlayer(plr, "You have successfully auth as SuperAdmin");
					}
				}
				else
				{
					Console.WriteLine("Unexpected message type!");
				}
			}
			catch(Exception ex)
			{
				CommandBoardcast.ShowError(ex);
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
					if (cmdIndex != -1)
					{
						Command cmd = Commands[cmdIndex];
						cmd.CommandAction(args);
					}
					else
					{
						Main.NewText("Command not found!", 255, 25, 0);
					}
				}
			}
		}

		private static void SetupDefaults()
		{
			//if (!Directory.Exists("Plugins"))
			//{
			//	Directory.CreateDirectory("Plugins");
			//}

			GroupType.SetupGroups();
			CommandDelegate.SetUpCommands(Commands);

			//物品信息读取方式添加
			ModDataHooks.BuildItemDataHook("prefix",
				(item) =>
				{
					return item.prefix.ToString();
				},
				(str, item) =>
				{
					item.prefix = Convert.ToByte(str);
				});
			ModDataHooks.BuildItemDataHook("stack",
				(item) =>
				{
					return item.stack.ToString();
				},
				(str, item) =>
				{
					item.stack = Convert.ToInt32(str);
				});
			ModDataHooks.BuildItemDataHook("fav",
				(item) =>
				{
					return item.favorited.ToString();
				},
				(str, item) =>
				{
					item.favorited = Convert.ToBoolean(str);
				});

			AddToStartInv(ItemID.ShadewoodSword, 82);
			AddToStartInv(ItemID.IronPickaxe);
			AddToStartInv(ItemID.IronAxe);
			if (ModLoader.LoadedMods.Any(mod => mod.Name == "ThoriumMod"))
			{
				var thorium = ModLoader.LoadedMods.Where(mod => mod.Name == "ThoriumMod");
				AddToStartInv(thorium.First().ItemType("FamilyHeirloom"));
			}
		}

		private static void AddToStartInv(int type, int prefex = 0)
		{
			Item item1 = new Item();
			item1.SetDefaults(type);
			item1.Prefix(prefex);
			ServerPlayer.StartUpItems.Add(item1);
		}

		private static ServerPlayer FindPlayer(string hash)
		{
			foreach(var pair in xmlData.Data)
			{
				if(pair.Value.Hash == hash)
				{
					return pair.Value;
				}
			}
			throw new Exception("Cannot find the player!");
		}

		private static double GetTime()
		{
			double time1 = Main.time / 3600.0;
			time1 += 4.5;
			if (!Main.dayTime)
				time1 += 15.0;
			time1 = time1 % 24.0;
			return time1;
		}

		public static bool CheckSpawn(int x, int y)
		{
			Vector2 tile = new Vector2(x, y);
			Vector2 spawn = new Vector2(Main.spawnTileX, Main.spawnTileY);
			return Vector2.Distance(spawn, tile) <= 5;
		}
	}
}