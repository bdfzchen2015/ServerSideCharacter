using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class GodCommand : ModCommand
	{
		public override string Command
		{
			get { return "god"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Toggle player's god mode"; }
		}

		public override string Usage
		{
			get { return "/god"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			ModPacket pack = ServerSideCharacter.Instance.GetPacket();
			pack.Write((int)SSCMessageType.ToggleGodMode);
			pack.Write((byte)Main.myPlayer);
			pack.Send();
		}
	}
}
