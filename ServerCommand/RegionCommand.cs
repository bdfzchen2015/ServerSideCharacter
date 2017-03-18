using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class RegionCommand : ModCommand
	{
		public override string Command
		{
			get { return "region"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Manage the regions in the server"; }
		}

		public override string Usage
		{
			get { return "/region <create/info/delete> [region name]"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args[0] == "create")
			{
				if ((ServerSideCharacter.TilePos1 == Vector2.Zero) || (ServerSideCharacter.TilePos2 == Vector2.Zero))
				{
					string name = args[1];
					MessageSender.SendRegionCreate(Main.myPlayer, name);
					ServerSideCharacter.TilePos1 = ServerSideCharacter.TilePos2 = Vector2.Zero;
				}
				else
				{
					Main.NewText("This is an invalid region", 255, 255, 0);
				}
			}
			else if (args[0] == "info")
			{

			}
			else if (args[0] == "delete")
			{
				string name = args[1];
				MessageSender.SendRegionRemove(Main.myPlayer, name);
			}
		}
	}
}
