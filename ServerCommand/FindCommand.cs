using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class FindCommand : ModCommand
	{
		public override string Command
		{
			get { return "find"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Find the ID of game entities (Item, NPC)"; }
		}

		public override string Usage
		{
			get { return "/find <type(-n/-i)> <string>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			string type = args[0];
			string tryToFind = args[1];
			if (type == "-i")
			{
				List<int> numbers = new List<int>(Main.itemName.Length);
				for (int i = 0; i < Main.itemName.Length; i++)
				{
					numbers.Add(i);
				}
				var items = numbers.Where(i => Main.itemName[i].ToLower().Contains(tryToFind.ToLower()));
				var enumerable = items as int[] ?? items.ToArray();
				foreach (var pair in enumerable)
				{
					Main.NewText(Main.itemName[pair] + " -> ID: " + pair, 255, 255, 50);
				}
				Main.NewText("Total Find: " + enumerable.Count());
			}
			else if (type == "-n")
			{
				List<int> numbers = new List<int>(Main.npcName.Length);
				for (int i = 0; i < Main.npcName.Length; i++)
				{
					numbers.Add(i);
				}
				var items = numbers.Where(i => Main.npcName[i].ToLower().Contains(tryToFind.ToLower()));
				var enumerable = items as IList<int> ?? items.ToList();
				foreach (var pair in enumerable)
				{
					Main.NewText(Main.npcName[pair] + " -> ID: " + pair, 255, 255, 50);
				}
				Main.NewText("Total Find: " + enumerable.Count());
			}
			else
			{
				Main.NewText("Invalid Sytanx! Usage: /find <type(-n/-i)> <string>", 255, 25, 0);
			}
		}
	}
}
