using ServerSideCharacter.Config;
using ServerSideCharacter.Region;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;
using Newtonsoft.Json;
using ServerSideCharacter.Extensions;
using System.Reflection;

namespace ServerSideCharacter
{
	public static class Utils
	{
		public static Player TryGetPlayer(string name)
		{
			try
			{
				return Main.player.ToList().Find(player => player != null && player.active && player.name.Contains(name, StringComparison.OrdinalIgnoreCase));
			}
			catch (Exception)
			{
				return null;
			}
		}
		private static Dictionary<int, NPC> npcs = new Dictionary<int, NPC>();
		private static void SetupNpcs()
		{
			Console.WriteLine(Main.NPCLoaded.Length);
			for (int i = 0; i < Main.NPCLoaded.Length; i++)
			{
				NPC npc = new NPC();
				npc.SetDefaults(i);
				npcs.Add(i, npc);
			}
		}
		public static NPC TryGetNPC(string name)
		{
			try
			{
				if (npcs.Count == 0)
					SetupNpcs();

				NPC[] npcArray = npcs.ToList().FindAll(v => v.Value.displayName.Contains(name, StringComparison.OrdinalIgnoreCase) || v.Value.name.Contains(name, StringComparison.OrdinalIgnoreCase)).Select(v => v.Value).ToArray();
				NPC best = null;
				if (npcArray.Length == 1)
					return npcArray[0];
				else if (npcArray.Length > 1)
					foreach (var npc in npcArray)
					{
						if (npc.name.EndsWith("body", StringComparison.OrdinalIgnoreCase) || npc.name.EndsWith("tail", StringComparison.OrdinalIgnoreCase) || npc.name.EndsWith("hand", StringComparison.OrdinalIgnoreCase) || npc.type == 396) // Ignore body, tail and hand and the Moon Lord Head (396). We only want the Head (or the Core in case of Moon Lord)
							continue;
						if (npc.name.Contains(name, StringComparison.OrdinalIgnoreCase) || npc.displayName.Contains(name, StringComparison.OrdinalIgnoreCase))
							best = npc;
						if (npc.name.Equals(name, StringComparison.OrdinalIgnoreCase) || npc.displayName.Equals(name, StringComparison.OrdinalIgnoreCase))
						{
							best = npc;
							break;
						}
					}
				else
					return null;
				return best == null ? npcArray[0] : best;
			}
			catch (Exception)
			{

				return null;
			}
		}
		public static NetItem ToNetItem(Item item)
		{
			NetItem toRet = new NetItem();
			if (item.modItem != null)
			{
				string nameSpace = item.modItem.GetType().Namespace;
				string itemName = item.modItem.GetType().Name;
				toRet.ItemID = -1;
				toRet.IsModItem = true;
				toRet.Prefix = item.prefix;
				toRet.ModName = nameSpace;
				toRet.ItemName = itemName;
				toRet.IsFavorite = item.favorited;
				return toRet;
			}
			else
			{
				toRet.ItemID = item.netID;
				toRet.IsModItem = false;
				toRet.Prefix = item.prefix;
				toRet.ModName = "";
				toRet.ItemName = "";
				toRet.IsFavorite = item.favorited;
				return toRet;
			}
		}

		public static NetItem ToNetItem(int type)
		{
			Item item = new Item();
			item.netDefaults(type);
			return ToNetItem(item);
		}

		public static Item GetItemFromNet(NetItem netItem)
		{
			if (netItem.IsModItem)
			{
				var target_mod = ModLoader.LoadedMods.Where(mod => mod.Name == netItem.ModName);
				if (target_mod.Count() == 0)
				{
					return new Item();
				}
				Item item = new Item();
				item.netDefaults(target_mod.First().ItemType(netItem.ItemName));
				item.prefix = (byte)netItem.Prefix;
				item.favorited = netItem.IsFavorite;
				return item;
			}
			else
			{
				Item item = new Item();
				item.netDefaults(netItem.ItemID);
				item.prefix = (byte)netItem.Prefix;
				item.favorited = netItem.IsFavorite;
				return item;
			}
		}

		public static ChestManager LoadChestInfo()
		{
			if (!File.Exists("SSC/chest.json"))
			{
				return new ChestManager().Initialize();
			}
			else
			{
				ChestManager manager;
				using (StreamReader sr = new StreamReader("SSC/chest.json"))
				{
					string data = sr.ReadToEnd();
					manager = JsonConvert.DeserializeObject<ChestManager>(data);
				}
				return manager;
			}
		}
		public static void SaveChestInfo()
		{
			string data = JsonConvert.SerializeObject(ServerSideCharacter.ChestManager, Formatting.None);
			File.WriteAllText("SSC/chest.json", data);
		}
	}
}
