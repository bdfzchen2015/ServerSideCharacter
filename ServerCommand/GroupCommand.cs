using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
				string hash = args[1];
				MessageSender.SendSetGroup(Main.myPlayer, hash, args[2]);
			}
			else if(args[0] == "append")
			{
				string name = args[1];
				string perm = args[2];
				throw new NotImplementedException();
			}
		}
	}
}
