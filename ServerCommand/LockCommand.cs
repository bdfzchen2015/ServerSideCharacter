using System;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class LockCommand : ModCommand
	{
		public override string Command
		{
			get { return "lock"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Lock a player"; }
		}

		public override string Usage
		{
			get { return "/lock <player id> <time>"; }
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
			MessageSender.SendLockCommand(Main.myPlayer, who, Convert.ToInt32(args[1]));
		}
	}
}
