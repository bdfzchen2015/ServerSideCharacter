using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class TeleportCommand : ModCommand
	{
		public override string Command
		{
			get { return "tp"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Teleport to a player"; }
		}

		public override string Usage
		{
			get { return "/tp <player id>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendTeleportCommand(Main.myPlayer, Convert.ToInt32(args[0]));
		}
	}
}
