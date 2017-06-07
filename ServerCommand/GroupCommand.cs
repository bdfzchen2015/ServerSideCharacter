using System;
using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class GroupCommand : ModCommand
	{
		public override string Command
		{
			get { return "group"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Manage player's group"; }
		}

		public override string Usage
		{
			get { return "/group [set | append] <$hash> <permission group | permission name>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args[0] == "set")
			{
				int uuid = int.Parse(args[1]);
				MessageSender.SendSetGroup(Main.myPlayer, uuid, args[2]);
			}
			else if (args[0] == "append")
			{
				string name = args[1];
				string perm = args[2];
				throw new NotImplementedException();
			}
		}
	}
}
