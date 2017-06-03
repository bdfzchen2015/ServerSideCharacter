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
			get { return " /sm <npc id|npc name> [number]"; }
		}
		static private string[] GetArgs(string[] source)
		{
			string name;
			int amount;
			if (source.Length > 1 && int.TryParse(source.Last(), out amount))
			{
				name = string.Join(" ", source.Take(source.Length - 1));
			}
			else
			{
				amount = 1;
				name = string.Join(" ", source);
			}
			return new string[2] { name, amount.ToString() };
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			args = GetArgs(args);
			int type;
			int amount = int.Parse(args[1]);
			if (!int.TryParse(args[0], out type))
			{
				NPC npc = Utils.TryGetNPC(args[0]);
				if (npc == null)
				{
					Main.NewText("NPC not found", Color.Red);
					return;
				}
				type = npc.type;
			}

			MessageSender.SendSummonCommand(Main.myPlayer, type, amount);
		}
	}
}
