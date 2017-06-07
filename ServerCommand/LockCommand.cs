using System;
using Terraria.ModLoader;
using Terraria;

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
			MessageSender.SendLockCommand(Main.myPlayer, Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
		}
	}
}
