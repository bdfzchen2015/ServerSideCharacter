using System;
using Terraria.ModLoader;

namespace ServerSideCharacter.ServerCommand
{
	public class ItemBanCommand : ModCommand
	{
		public override string Command
		{
			get { return "banitem"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Toggle ban items"; }
		}

		public override string Usage
		{
			get { return "/banitem <Item ID>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendBanItemCommand(Convert.ToInt32(args[0]));
		}
	}
}
