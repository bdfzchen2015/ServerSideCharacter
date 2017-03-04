using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public static class CommandDelegate
	{
		public static void SetUpCommands(List<Command> list)
		{
			list.Clear();
			list.Add(new Command("save", SaveCommand));
			list.Add(new Command("register", RegisterCommand));
			list.Add(new Command("login", LoginCommand));
			list.Add(new Command("kill", KillCommand));
			list.Add(new Command("ls", ListCommand));
			list.Add(new Command("group", GroupCommand));
			list.Add(new Command("lock", LockCommand));
			list.Add(new Command("butcher", ButcherCommand));
			list.Add(new Command("tp", TeleportCommand));
			list.Add(new Command("time", TimeCommand));
			list.Add(new Command("help", HelpCommand));
			list.Add(new Command("region", RegionCommand));
			list.Add(new Command("item", ItemCommand));
			list.Add(new Command("find", FindCommand));
			list.Add(new Command("auth", AuthCommand));
		}

		private static void AuthCommand(string[] obj)
		{
			try
			{
				NetSync.SendAuthRequest(Main.myPlayer, obj[0]);
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
				string trytoFind = obj[0];
				List<int> numbers = new List<int>(Main.maxItemTypes);
				for(int i = 0; i < Main.maxItemTypes; i++)
				{
					numbers.Add(i);
				}
				var items = numbers.Where(i => Main.itemName[i].ToLower().Contains(trytoFind.ToLower()));
				foreach(var pair in items)
				{
					Main.NewText(Main.itemName[pair] + " -> " + pair);
				}

			}
			catch
			{
				Main.NewText("Invalid Sytanx! Usage: /tp <player id>", 255, 25, 0);
			}
		}

		private static void SaveCommand(string[] args)
		{
			NetSync.SendRequestSave(Main.myPlayer);
		}
		private static void RegisterCommand(string[] args)
		{
			try
			{
				NetSync.SendSetPassword(Main.myPlayer, args[0]);
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
				NetSync.SendLoginPassword(Main.myPlayer, args[0]);
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
				NetSync.SendKillCommand(Main.myPlayer, Convert.ToInt32(args[0]));
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
				NetSync.SendTeleportCommand(Main.myPlayer, Convert.ToInt32(args[0]));
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
					NetSync.SendTimeCommand(Main.myPlayer, false, 0, true);
					return;
				}
				else
				{

					switch (args[0])
					{
						case "night":
							NetSync.SendTimeCommand(Main.myPlayer, true, 0, false);
							break;
						case "morning":
							NetSync.SendTimeCommand(Main.myPlayer, true, 0, true);
							break;
						case "noon":
							NetSync.SendTimeCommand(Main.myPlayer, true, 27000, true);
							break;
						case "midnight":
							NetSync.SendTimeCommand(Main.myPlayer, true, 16200, false);
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
			NetSync.SendListCommand(Main.myPlayer);
		}

		private static void ButcherCommand(string[] args)
		{
			NetSync.SendButcherCommand(Main.myPlayer);
		}

		private static void HelpCommand(string[] args)
		{
			NetSync.SendHelpCommand(Main.myPlayer);
		}

		private static void LockCommand(string[] args)
		{
			try
			{
				NetSync.SendLockCommand(Main.myPlayer, Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
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
				NetSync.SendItemCommand(Convert.ToInt32(args[0]));
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
					NetSync.SendSetGroup(Main.myPlayer, hash, args[2]);
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
					//string name = args[1];
					//NetSync.SendSetRegion(Main.myPlayer, name);
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
