using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Terraria;
using ServerSideCharacter.Extensions;

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
			get { return "/find <type(-n/-i/-p)> <string>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			string type = args[0];
			string tryToFind = args[1];
			if (type == "-i")
			{
				List<int> numbers = new List<int>(Main.maxItemTypes);
				for (int i = 0; i < Main.maxItemTypes; i++)
				{
					numbers.Add(i);
				}
				var items = numbers.Where(i => Lang.GetItemNameValue(i).Contains(tryToFind, StringComparison.OrdinalIgnoreCase));
				var enumerable = items as int[] ?? items.ToArray();
				foreach (var pair in enumerable)
				{
					Main.NewText(Lang.GetItemNameValue(pair) + " -> ID: " + pair, 255, 255, 50);
				}
				Main.NewText("Total Find: " + enumerable.Length);
			}
			else if (type == "-n")
			{
				List<int> numbers = new List<int>(Main.maxNPCTypes);
				for (int i = 0; i < Main.maxNPCTypes; i++)
				{
					numbers.Add(i);
				}
				var items = numbers.Where(i => Lang.GetNPCNameValue(i).Contains(tryToFind, StringComparison.OrdinalIgnoreCase));
				var enumerable = items as IList<int> ?? items.ToList();
				foreach (var pair in enumerable)
				{
					Main.NewText(Lang.GetNPCNameValue(pair) + " -> ID: " + pair, 255, 255, 50);
				}
				Main.NewText("Total Find: " + enumerable.Count());
			}
			else if (type == "-p")
			{
				var players = Main.player.Where(p => p.active &&
					p.name.Contains(tryToFind, StringComparison.OrdinalIgnoreCase)).ToList();
				foreach (var player in players)
				{
					Main.NewText(player.name + " -> ID: " + player.whoAmI, 255, 255, 50);
				}
			}
			else
			{
				Main.NewText("Invalid Sytanx! Usage: /find <type(-n/-i/-p)> <string>", 255, 25, 0);
			}
		}
	}
}
