using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class ListCommand : ModCommand
	{
		public override string Command
		{
			get { return "ls"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "List the data of the server"; }
		}

		public override string Usage
		{
			get { return "/ls [-al] [-rg / -gp]"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			bool all = false;
			ListType type = ListType.ListPlayers;
			if (args.Length > 0)
			{
				if (args.Any(str => str.Equals("-al")))
				{
					all = true;
				}
				if (args[0] == "-rg")
				{
					type = ListType.ListRegions;
				}
				else if(args[0] == "-gp")
				{
					type = ListType.ListGroups;
				}
			}
			MessageSender.SendListCommand(Main.myPlayer, type, all);
		}
	}
}
