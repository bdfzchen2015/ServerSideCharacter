using ServerSideCharacter.Config;
using ServerSideCharacter.Region;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Newtonsoft.Json;
using ServerSideCharacter.Extensions;
using System.Text;

namespace ServerSideCharacter
{
	public static class Utils
	{
		public static string[] ParseArgs(string[] args) // Parse commands args. Example: ' /chest addfriend "player name" ' is parsed into {"addfriend", "player name"} instead of {"addfriend", "player", "name"}
		{
			List<string> parsedArgs = new List<string>();
			string argsString = string.Join(" ", args).Trim();
			bool quotes = false;
			bool forceAppend = false;
			StringBuilder arg = new StringBuilder();
			for (int i = 0; i < argsString.Length; i++)
			{
				char c = argsString[i];
				switch (c)
				{
					case '\\':
						if (forceAppend)
						{
							arg.Append(c);
							forceAppend = false;
						}
						else
							forceAppend = true;
						continue;
					case '"':
						if (forceAppend)
						{
							arg.Append(c);
							forceAppend = false;
							continue;
						}
						if (quotes && !forceAppend)
						{
							quotes = false;
							parsedArgs.Add(arg.ToString());
							arg.Clear();
						}
						else if (forceAppend)
						{
							arg.Append(c);
							forceAppend = false;
						}
						else
							quotes = true;
						continue;
					case ' ':
						if (quotes)
						{
							arg.Append(c);
							continue;
						}
						else
						{
							parsedArgs.Add(arg.ToString());
							arg.Clear();
						}
						break;
					default:
						arg.Append(c);
						break;
				}
			}
			if (arg.Length > 0)
				parsedArgs.Add(arg.ToString());
			return parsedArgs.Where(a => a != "").ToArray();
		}
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
		public static int TryGetPlayerID(object obj)
		{
			if (!(obj is int) && !(obj is string) && !(obj is byte))
				return -1;
			string text = obj.ToString();
			int who;
			if (!int.TryParse(text, out who))
			{
				Player player = Utils.TryGetPlayer(text);
				if (player == null || !player.active)
				{
					return -1;
				}
				return player.whoAmI;
			}
			else
			{
				if (who < 0 || who > Main.maxPlayers)
					return -1;
				Player player = Main.player[who];
				if (player == null || !player.active)
					return -1;
				return who;
			}
		}
		private static Dictionary<int, NPC> npcs = new Dictionary<int, NPC>();
		private static readonly int[] ignoredNpcs = { 8, 9, 11, 12, 14, 15, 36, 40, 41, 89, 90, 91, 92, 96, 97, 99, 100, 118, 119, 128, 129, 130, 131, 135, 136, 246, 247, 248, 261, 263, 264, 265, 394, 396, 397, 403, 404, 413, 414, 455, 456, 457, 458, 459, 492, 511, 512, 514, 515 };
		private static void SetupNpcs()
		{
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

				NPC[] npcArray = npcs.ToList().FindAll(v => v.Value.TypeName.Contains(name, StringComparison.OrdinalIgnoreCase) || v.Value.TypeName.Contains(name, StringComparison.OrdinalIgnoreCase)).Select(v => v.Value).ToArray();
				NPC best = null;
				if (npcArray.Length == 1)
					return npcArray[0];
				else if (npcArray.Length > 1)
					foreach (var npc in npcArray)
					{
						if (ignoredNpcs.Contains(npc.type))
							continue;
						if (npc.TypeName.Contains(name, StringComparison.OrdinalIgnoreCase))
							best = npc;
						if (npc.TypeName.Equals(name, StringComparison.OrdinalIgnoreCase))
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
