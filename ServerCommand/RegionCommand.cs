using System;
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
			get { return "/region <create/share/delete> [region name]\nIf using 'share' you should provide player's id"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args[0] == "create")
			{
				if ((ServerSideCharacter.TilePos1 != Vector2.Zero) && (ServerSideCharacter.TilePos2 != Vector2.Zero))
				{
					string name = args[1];
					MessageSender.SendRegionCreate(Main.myPlayer, name);
					ServerSideCharacter.TilePos1 = ServerSideCharacter.TilePos2 = Vector2.Zero;
				}
				else
				{
					Main.NewText("Region position is invalid or you haven't select any region", 255, 255, 0);
				}
			}
			else if (args[0] == "share")
			{
				string name = args[1];
				int id = Convert.ToByte(args[2]);
				if (id != Main.myPlayer)
				{
					MessageSender.SendRegionShare(Main.myPlayer, name, id);
				}
				else
				{
					Main.NewText("You cannot share region with yourself", 255, 255, 0);
				}
			}
			else if (args[0] == "delete")
			{
				string name = args[1];
				MessageSender.SendRegionRemove(Main.myPlayer, name);
			}
		}
	}
}
