using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class InfoCommand : ModCommand
	{
		public override string Command
		{
			get { return "info"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Get the info of this mod"; }
		}

		public override string Usage
		{
			get { return "/info"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			Main.NewText("mod作者: DXTsT\n当前版本: " + ServerSideCharacter.APIVersion + "\nGithub网址: https://github.com/bdfzchen2015/ServerSideCharacter" +
				"\n感谢你们对我的支持!", Color.Yellow);
		}
	}
}
