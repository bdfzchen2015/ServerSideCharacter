using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class SummonCommand : ModCommand
	{
		public override string Command
		{
			get { return "sm"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Summon NPCs"; }
		}

		public override string Usage
		{
			get { return " /sm <npc id> <number>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendSummonCommand(Main.myPlayer, Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
		}
	}
}
