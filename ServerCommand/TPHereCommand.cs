using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class TPHereCommand : ModCommand
	{
		public override string Command
		{
			get { return "tphere"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Teleport a player to your position"; }
		}

		public override string Usage
		{
			get { return "/tphere <player id|player name>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			int who;
			if (!int.TryParse(args[0], out who))
			{
				Player player = Utils.TryGetPlayer(args[0]);
				if (player == null || !player.active)
				{
					Main.NewText("Player not found", Color.Red);
					return;
				}
				who = player.whoAmI;
			}
			ModPacket pack = ServerSideCharacter.Instance.GetPacket();
			pack.Write((int)SSCMessageType.TPHereCommand);
			pack.Write((byte)Main.myPlayer);
			pack.Write((byte)who);
			pack.Send();
		}
	}
}
