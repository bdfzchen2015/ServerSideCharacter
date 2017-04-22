using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;
using ServerSideCharacter.GroupManage;

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
				else if (args[0] == "-gp")
				{
					type = ListType.ListGroups;
				}
			}
			MessageSender.SendListCommand(Main.myPlayer, type, all);
		}
	}
	public class ListCommandConsole : ModCommand
	{
		public override string Command
		{
			get { return "ls"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Console; }
		}

		public override string Description
		{
			get { return "List the data of the server"; }
		}

		public override string Usage
		{
			get { return "/ls [rg / gp]"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			try
			{
				StringBuilder sb = new StringBuilder();
				if (args.Length > 0)
				{
					if (args[0] == "rg")
					{
						sb.AppendLine("RegionName\tOwner\tRegion Area");
						foreach (var region in ServerSideCharacter.RegionManager.ServerRegions)
						{
							string line = string.Concat(
								region.Name,
								"\t",
								region.Owner.Name,
								"\t",
								region.Area.ToString()
								);
							sb.AppendLine(line);
						}
					}
					else if (args[0] == "gp")
					{
						int i = 1;
						foreach (var group in GroupType.Groups)
						{
							sb.AppendLine(string.Format("{0}. Group Name: {1}  Chat Prefix: {2}\n   Permissions:",
								i, group.Key, group.Value.ChatPrefix));
							sb.AppendLine("{");
							foreach (var perm in group.Value.permissions)
							{
								sb.AppendLine("\t" + perm.Name);
							}
							sb.AppendLine("}");
							i++;
						}
					}
					else
					{
						CommandBoardcast.ConsoleError("Invalid Sytanx: ls [rg / gp]");
					}
				}
				else
				{
					sb.AppendLine("Player ID\tName\tUUID\tPermission\tGroup\tLifeMax");
					foreach (var pla in ServerSideCharacter.XmlData.Data)
					{
						Player player1 = pla.Value.PrototypePlayer;
						string line = string.Concat(
							player1 != null && player1.active ? player1.whoAmI.ToString() : "N/A",
							"\t",
							pla.Value.Name,
							"\t",
							pla.Value.UUID,
							"\t",
							pla.Value.PermissionGroup.GroupName,
							"\t",
							pla.Value.LifeMax,
							"\t"
							);
						sb.AppendLine(line);
					}
				}
				Console.WriteLine(sb.ToString());
			}
			catch (Exception ex)
			{
				CommandBoardcast.ConsoleError(ex);
			}
		}
	}
}
