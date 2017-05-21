using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace ServerSideCharacter.Config
{
	public class NetItem
	{
		public int ItemID;
		public int Prefix;
		public bool IsModItem;
		public bool IsFavorite;
		public string ModName;
		public string ItemName;

		public bool TheSameItem(Item item)
		{
			if(item.modItem != null)
			{
				NetItem tmp = Utils.ToNetItem(item);
				return tmp.ModName == this.ModName && tmp.ItemName == this.ItemName;
			}
			else
			{
				return item.type == ItemID;
			}
		}
	}
}
