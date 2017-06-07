using System;
using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class KillCommand : ModCommand
	{
		public override string Command
		{
			get { return "kill"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Kill a player"; }
		}

		public override string Usage
		{
			get { return "/kill <player id>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendKillCommand(Main.myPlayer, Convert.ToInt32(args[0]));
		}
	}
}
