using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class RegisterCommand : ModCommand
	{
		public override string Command
		{
			get { return "register"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Register for an account"; }
		}

		public override string Usage
		{
			get { return "/register <your password>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendSetPassword(Main.myPlayer, args[0]);
		}
	}
}
