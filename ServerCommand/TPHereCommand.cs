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
			get { return "/tphere <player id>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			ModPacket pack = ServerSideCharacter.Instance.GetPacket();
			pack.Write((int)SSCMessageType.TPHereCommand);
			pack.Write((byte)Main.myPlayer);
			pack.Write(Convert.ToByte(args[0]));
			pack.Send();
		}
	}
}
