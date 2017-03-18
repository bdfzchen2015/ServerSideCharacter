using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class TimeCommand : ModCommand
	{
		public override string Command
		{
			get { return "time"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Change the Time of the server"; }
		}

		public override string Usage
		{
			get { return "/time [night/morning/noon/midnight]"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (args.Length == 0)
			{
				MessageSender.SendTimeCommand(Main.myPlayer, false, 0, true);
			}
			else
			{

				switch (args[0])
				{
					case "night":
						MessageSender.SendTimeCommand(Main.myPlayer, true, 0, false);
						break;
					case "morning":
						MessageSender.SendTimeCommand(Main.myPlayer, true, 0, true);
						break;
					case "noon":
						MessageSender.SendTimeCommand(Main.myPlayer, true, 27000, true);
						break;
					case "midnight":
						MessageSender.SendTimeCommand(Main.myPlayer, true, 16200, false);
						break;
					default:
						Main.NewText("Invalid Sytanx! Usage: /time [night/morning/noon/midnight]", 255, 25, 0);
						break;
				}
			}
		}
	}
}
