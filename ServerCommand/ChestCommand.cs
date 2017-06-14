using System;
using Terraria.ModLoader;
using Terraria;
using ServerSideCharacter.Region;

namespace ServerSideCharacter.ServerCommand
{
	public class ChestCommand : ModCommand
	{
		public override string Command
		{
			get { return "chest"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Chest related commands"; }
		}

		public override string Usage
		{
			get { return "/chest <addfriend|removefriend> <name>\n/chest <public|unpublic>\n/chest <protect|deprotect>\n/chest info"; }
		}
		public override void Action(CommandCaller caller, string input, string[] args)
		{
			args = Utils.ParseArgs(args);
			ChestManager.Pending pending = (ChestManager.Pending)Enum.Parse(typeof(ChestManager.Pending), args[0], true);
			int plr = Main.myPlayer;
			string friend = args.Length > 1 ? args[1] : null;
			MessageSender.SendChestCommand(pending, plr, friend);
		}
	}
}
