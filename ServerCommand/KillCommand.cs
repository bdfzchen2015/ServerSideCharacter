using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

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
			args = Utils.ParseArgs(args);
			int who = Utils.TryGetPlayerID(args[0]);
			if (who == -1)
			{
				Main.NewText("Player not found", Color.Red);
				return;
			}
			if (who == Main.myPlayer)
			{
				Main.NewText("You cannot kill yourself", Color.Red);
				return;
			}
			MessageSender.SendKillCommand(Main.myPlayer, who);
		}
	}
}
