using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace ServerSideCharacter.ServerCommand
{
	public enum ListType
	{
		ListPlayers,
		ListRegions
	}

	public static class CommandDelegate
	{
		public static void SetUpCommands(List<Command> list)
		{
			list.Clear();
			list.Add(new Command("save", SaveCommand, "Save player's data"));
			list.Add(new Command("register", RegisterCommand, "Register an account"));
			list.Add(new Command("login", LoginCommand, "Login to an account"));
			list.Add(new Command("kill", KillCommand, "Kill a player"));
			list.Add(new Command("ls", ListCommand, "List current player's info"));
			list.Add(new Command("group", GroupCommand, "Group management"));
			list.Add(new Command("lock", LockCommand, "Lock a player"));
			list.Add(new Command("butcher", ButcherCommand, "Butcher all hostile npcs"));
			list.Add(new Command("tp", TeleportCommand, "Teleport to a player"));
			list.Add(new Command("time", TimeCommand, "Check or adjust the time of the world"));
			list.Add(new Command("help", HelpCommand, "Show commands"));
			list.Add(new Command("region", RegionCommand, "Region management"));
			list.Add(new Command("item", ItemCommand, "Give player items"));
			list.Add(new Command("find", FindCommand, "Find item(-i) or npc(-n) names"));
			list.Add(new Command("auth", AuthCommand, "Aithorize command"));
			list.Add(new Command("sm", SummonCommand, "Summon npcs"));
			list.Add(new Command("tphere", TPHereCommand, "Teleport a player to your position"));
			list.Add(new Command("god", GodCommand, "Toggle player's god mode"));
		}

		private static void GodCommand(string[] obj)
		{
			ModPacket pack = ServerSideCharacter.instance.GetPacket();
			pack.Write((int)SSCMessageType.ToggleGodMode);
			pack.Write(Main.myPlayer);
			pack.Send();
		}

		private static void TPHereCommand(string[] obj)
		{
			try
			{
				ModPacket pack = ServerSideCharacter.instance.GetPacket();
				pack.Write((int)SSCMessageType.TPHereCommand);
				pack.Write((byte)Main.myPlayer);
				pack.Write(Convert.ToByte(obj[0]));
				pack.Send();
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /tphere <player id>", 255, 25, 0);
			}
		}

		private static void SummonCommand(string[] args)
		{
			try
			{
				MessageSender.SendSummonCommand(Main.myPlayer, Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /sm <npc id> <number>", 255, 25, 0);
			}
		}

		private static void AuthCommand(string[] obj)
		{
			try
			{
				MessageSender.SendAuthRequest(Main.myPlayer, obj[0]);
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /auth <code>", 255, 25, 0);
			}
		}

		private static void FindCommand(string[] obj)
		{
			try
			{
				string type = obj[0];
				string tryToFind = obj[1];
				if (type == "-i")
				{
					List<int> numbers = new List<int>(Main.itemName.Length);
					for (int i = 0; i < Main.itemName.Length; i++)
					{
						numbers.Add(i);
					}
					var items = numbers.Where(i => Main.itemName[i].ToLower().Contains(tryToFind.ToLower()));
					foreach (var pair in items)
					{
						Main.NewText(Main.itemName[pair] + " -> ID: " + pair);
					}
					Main.NewText("Total Find: " + items.Count());
				}
				else if(type == "-n")
				{
					List<int> numbers = new List<int>(Main.npcName.Length);
					for (int i = 0; i < Main.npcName.Length; i++)
					{
						numbers.Add(i);
					}
					var items = numbers.Where(i => Main.npcName[i].ToLower().Contains(tryToFind.ToLower()));
					foreach (var pair in items)
					{
						Main.NewText(Main.npcName[pair] + " -> ID: " + pair);
					}
					Main.NewText("Total Find: " + items.Count());
				}
				else
				{
					Main.NewText("Invalid Sytanx! Usage: /find <type(-n/-i)> <string>", 255, 25, 0);
				}

			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /find <type(-n/-i)> <string>", 255, 25, 0);
			}
		}

		private static void SaveCommand(string[] args)
		{
			MessageSender.SendRequestSave(Main.myPlayer);
			Main.NewText("Saved player's data");
		}
		private static void RegisterCommand(string[] args)
		{
			try
			{
				MessageSender.SendSetPassword(Main.myPlayer, args[0]);
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /register <your password>", 255, 255, 0);
			}
		}

		private static void LoginCommand(string[] args)
		{
			try
			{
				MessageSender.SendLoginPassword(Main.myPlayer, args[0]);
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /login <your password>", 255, 255, 0);
			}
		}

		private static void KillCommand(string[] args)
		{
			try
			{
				MessageSender.SendKillCommand(Main.myPlayer, Convert.ToInt32(args[0]));
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /kill <player id>", 255, 25, 0);
			}
		}

		private static void TeleportCommand(string[] args)
		{
			try
			{
				MessageSender.SendTeleportCommand(Main.myPlayer, Convert.ToInt32(args[0]));
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /tp <player id>", 255, 25, 0);
			}
		}

		private static void TimeCommand(string[] args)
		{
			try
			{
				if (args.Length == 0)
				{
					MessageSender.SendTimeCommand(Main.myPlayer, false, 0, true);
					return;
				}
				else
				{

					switch (args[0])
					{
						case "night":
							MessageSender.SendTimeCommand(Main.myPlayer, true, 0, false);
							break;
						case "morning":
							MessageSender.SendTimeCommand(Main.myPlayer, true, 0, true);
							break;
						case "noon":
							MessageSender.SendTimeCommand(Main.myPlayer, true, 27000, true);
							break;
						case "midnight":
							MessageSender.SendTimeCommand(Main.myPlayer, true, 16200, false);
							break;
						default:
							Main.NewText("Invalid Sytanx! Usage: /time [night/morning/noon/midnight]", 255, 25, 0);
							break;
					}
				}
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /time [night/morning/noon/midnight]", 255, 25, 0);
			}
		}

		private static void ListCommand(string[] args)
		{
			try
			{
				bool all = false;
				ListType type = ListType.ListPlayers;
				if (args.Length > 0)
				{
					if (args.Any(str => str.Equals("-al")))
					{
						all = true;
					}
					if (args[0] == "-rg")
					{
						type = ListType.ListRegions;
					}
				}
				MessageSender.SendListCommand(Main.myPlayer, type, all);
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /ls [-al]", 255, 25, 0);
			}
		}

		private static void ButcherCommand(string[] args)
		{
			MessageSender.SendButcherCommand(Main.myPlayer);
		}

		private static void HelpCommand(string[] args)
		{
			MessageSender.SendHelpCommand(Main.myPlayer);
		}

		private static void LockCommand(string[] args)
		{
			try
			{
				MessageSender.SendLockCommand(Main.myPlayer, Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /lock <player id> <time>", 255, 25, 0);
			}
		}

		private static void ItemCommand(string[] args)
		{
			try
			{
				MessageSender.SendItemCommand(Convert.ToInt32(args[0]));
			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /item <type>", 255, 25, 0);
			}
		}

		private static void GroupCommand(string[] args)
		{
			try
			{
				if (args[0] == "set")
				{
					string hash = args[1];
					MessageSender.SendSetGroup(Main.myPlayer, hash, args[2]);
				}
			}
			catch(Exception ex)
			{
				Main.NewText("Invalid Sytanx! Usage: /group set <$hash> <permission group>", 255, 25, 0);
			}
		}

		private static void RegionCommand(string[] args)
		{
			try
			{
				if (args[0] == "create")
				{
					string name = args[1];
					MessageSender.SendRegionCreate(Main.myPlayer, name);
				}
				else if(args[0] == "info")
				{

				}
				else if (args[0] == "delete")
				{

				}
			}
			catch (Exception ex)
			{
				Main.NewText("Invalid Sytanx! Usage: /region <create/info/delete> [region name]", 255, 25, 0);
			}
		}
	}
}
