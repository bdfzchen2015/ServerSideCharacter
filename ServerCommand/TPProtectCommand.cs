using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class TPProtectCommand : ModCommand
	{
		public override string Command
		{
			get { return "tpprotect"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Toggle teleportation protect"; }
		}

		public override string Usage
		{
			get { return "/tpprotect"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendTPProtect(Main.myPlayer);
		}
	}
}
