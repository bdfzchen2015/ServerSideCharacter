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
	}
}
