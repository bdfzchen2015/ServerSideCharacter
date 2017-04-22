using ServerSideCharacter.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace ServerSideCharacter
{
	public static class Utils
	{
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
				return toRet;
			}
			else
			{
				toRet.ItemID = item.netID;
				toRet.IsModItem = false;
				toRet.Prefix = item.prefix;
				toRet.ModName = "";
				toRet.ItemName = "";
				return toRet;
			}
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
				return item;
			}
			else
			{
				Item item = new Item();
				item.netDefaults(netItem.ItemID);
				item.prefix = (byte)netItem.Prefix;
				return item;
			}
		}
	}
}
