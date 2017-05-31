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

namespace ServerSideCharacter
{
	public static class Utils
	{
		public static Player TryGetPlayer(string name)
		{
			try
			{
				return Main.player.ToList().Find(player => player != null && player.active && player.name.Equals(name, StringComparison.OrdinalIgnoreCase));
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
			if(netItem.IsModItem)
			{
				var target_mod = ModLoader.LoadedMods.Where(mod => mod.Name == netItem.ModName);
				if(target_mod.Count() == 0)
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
				using(StreamReader sr = new StreamReader("SSC/chest.json"))
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
