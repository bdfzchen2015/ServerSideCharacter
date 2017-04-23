using Microsoft.Xna.Framework;
using ServerSideCharacter.GroupManage;
using ServerSideCharacter.Region;
using ServerSideCharacter.ServerCommand;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;

namespace ServerSideCharacter
{
	public delegate bool MessagePatchDelegate(ref BinaryReader reader, int playerNumber);

	public class MessageChecker
	{
		private Dictionary<int, MessagePatchDelegate> _method;

		public bool CheckMessage(ref byte messageType, ref BinaryReader reader, int playerNumber)
		{
			try
			{
				if (_method.ContainsKey(messageType))
				{
					return _method[messageType](ref reader, playerNumber);
				}
			}
			catch(Exception ex)
			{
				CommandBoardcast.ConsoleError(ex);
			}

			return false;
		}

		public MessageChecker()
		{
			_method = new Dictionary<int, MessagePatchDelegate>
			{
				{ MessageID.SpawnPlayer, PlayerSpawn },
				{ MessageID.ChatText, ChatText },
				{ MessageID.TileChange, TileChange },
				{ MessageID.PlayerControls, PlayerControls },
				{ MessageID.RequestChestOpen, RequestChestOpen }
			};
		}

		private bool RequestChestOpen(ref BinaryReader reader, int playerNumber)
		{
			if (Main.netMode == 2)
			{
				int x = reader.ReadInt16();
				int y = reader.ReadInt16();
				int id = Chest.FindChest(x, y);
				Player player = Main.player[playerNumber];
				ServerPlayer sPlayer = player.GetServerPlayer();
				if (ServerSideCharacter.ChestManager.IsNull(id))
				{
					ServerSideCharacter.ChestManager.SetOwner(id, playerNumber);
				}
				else if (ServerSideCharacter.ChestManager.CanOpen(id, sPlayer))
				{
					return false;
				}
				else
				{
					sPlayer.SendErrorInfo("You cannot open this chest");
				}
			}
			return true;
		}

		private bool PlayerControls(ref BinaryReader reader, int playerNumber)
		{
			if (Main.netMode == 2)
			{
				byte plr = reader.ReadByte();
				BitsByte control = reader.ReadByte();
				BitsByte pulley = reader.ReadByte();
				byte item = reader.ReadByte();
				var pos = reader.ReadVector2();
				Player player = Main.player[playerNumber];
				ServerPlayer sPlayer = player.GetServerPlayer();
				if (pulley[2])
				{
					var vel = reader.ReadVector2();
				}
				if (ServerSideCharacter.Config.IsItemBanned(sPlayer.PrototypePlayer.inventory[item], sPlayer))
				{
					sPlayer.ApplyLockBuffs();
					sPlayer.SendErrorInfo("You used a banned item: " + player.inventory[item].name);
				}
			}
			return false;
		}

		private bool TileChange(ref BinaryReader reader, int playerNumber)
		{
			if (Main.netMode == 2)
			{
				try
				{
					Player p = Main.player[playerNumber];
					ServerPlayer player = p.GetServerPlayer();
					int action = reader.ReadByte();
					short X = reader.ReadInt16();
					short Y = reader.ReadInt16();
					short type = reader.ReadInt16();
					int style = reader.ReadByte();
					if (ServerSideCharacter.CheckSpawn(X, Y) && player.PermissionGroup.GroupName != "spadmin")
					{
						player.SendErrorInfo("Warning: Spawn is protected from change");
						NetMessage.SendTileSquare(-1, X, Y, 4);
						return true;
					}
					else if (ServerSideCharacter.RegionManager.CheckRegion(X, Y, player))
					{
						player.SendErrorInfo("Warning: You don't have permission to change this tile");
						NetMessage.SendTileSquare(-1, X, Y, 4);
						return true;
					}
					else if (player.PermissionGroup.GroupName == "criminal")
					{
						player.SendErrorInfo("Warning: Criminals cannot change tiles");
						NetMessage.SendTileSquare(-1, X, Y, 4);
						return true;
					}
				}
				catch (Exception ex)
				{
					CommandBoardcast.ConsoleError(ex);
				}
			}
			return false;
		}

		private bool ChatText(ref BinaryReader reader, int playerNumber)
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
				Group group = player.PermissionGroup;
				string prefix = "[" + group.ChatPrefix + "] ";
				c = group.ChatColor;
				NetMessage.SendData(25, -1, -1, prefix + "<" + p.name + "> " + text, playerID, (float)c.R, (float)c.G, (float)c.B, 0, 0, 0);
				if (Main.dedServ)
				{
					Console.WriteLine("{0}<" + Main.player[playerID].name + "> " + text, prefix);
				}
			}
			return true;
		}

		private bool PlayerSpawn(ref BinaryReader reader, int playerNumber)
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
			if (!ServerSideCharacter.XmlData.Data.ContainsKey(Main.player[playerNumber].name))
			{
				try
				{
					//创建新的玩家数据
					ServerPlayer serverPlayer = ServerPlayer.CreateNewPlayer(Main.player[playerNumber]);
					serverPlayer.PrototypePlayer = Main.player[playerNumber];
					ServerSideCharacter.XmlData.Data.Add(Main.player[playerNumber].name, serverPlayer);
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
				ServerSideCharacter.SyncConnectedPlayer(playerNumber);
				NetMessage.SendData(MessageID.SpawnPlayer, -1, playerNumber, "", playerNumber, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData(MessageID.AnglerQuest, playerNumber, -1, Main.player[playerNumber].name, Main.anglerQuest, 0f, 0f, 0f, 0, 0, 0);
				return true;
			}
			NetMessage.SendData(MessageID.SpawnPlayer, -1, playerNumber, "", playerNumber, 0f, 0f, 0f, 0, 0, 0);
			return true;
		}
	}
}
