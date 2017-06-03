using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace ServerSideCharacter.ServerCommand
{
	public class KeyCommand : ModCommand
	{
		private static HashSet<int> keys = new HashSet<int>()
		{
			ItemID.HallowedKey,
			ItemID.JungleKey,
			ItemID.CrimsonKey,
			ItemID.CorruptionKey,
			ItemID.FrozenKey,
			ItemID.GoldenKey
		};

		public override string Command
		{
			get { return "key"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Use your biome keys to get weapons"; }
		}

		public override string Usage
		{
			get { return "/key"; }
		}

		private int GetKeyItem(int id)
		{
			if (id == ItemID.JungleKey)
			{
				return ItemID.PiranhaGun;
			}
			else if (id == ItemID.HallowedKey)
			{
				return ItemID.RainbowGun;
			}
			else if (id == ItemID.CrimsonKey)
			{
				return ItemID.VampireKnives;
			}
			else if (id == ItemID.CorruptionKey)
			{
				return ItemID.ScourgeoftheCorruptor;
			}
			else if (id == ItemID.FrozenKey)
			{
				return ItemID.StaffoftheFrostHydra;
			}
			else if (id == ItemID.GoldenKey)
			{
				int i = Main.rand.Next(5);
				if (i == 0) return ItemID.Handgun;
				if (i == 1) return ItemID.CobaltShield;
				if (i == 2) return ItemID.Muramasa;
				if (i == 3) return ItemID.AquaScepter;
				if (i == 4) return ItemID.BlueMoon;
			}
			return 0;
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			var player = caller.Player;
			bool anyKeys = false;
			for (int i = 0; i < player.inventory.Length; i++)
			{
				if (keys.Contains(player.inventory[i].type))
				{
					while (player.inventory[i].stack != 0)
					{
						player.inventory[i].stack--;
						player.QuickSpawnItem(GetKeyItem(player.inventory[i].type));
					}
					player.inventory[i] = new Item();
					anyKeys = true;
				}
			}
			if (!anyKeys)
			{
				Main.NewText("No key in your inventory!", 255, 255, 0);
			}
			else
			{
				Main.NewText("Exchange weapons succeed!", 40, 255, 40);
			}
		}
	}
}
