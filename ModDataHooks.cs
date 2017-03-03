using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace ServerSideCharacter
{
	public class ModDataHooks
	{
		public static Dictionary<string, Func<Item, string>> ItemExtraInfoTable = new Dictionary<string, Func<Item, string>>();
		public static Dictionary<string, Action<string, Item>> InterpretStringTable = new Dictionary<string, Action<string, Item>>();

		public static void BuildItemDataHook(string key, Func<Item, string> write, Action<string, Item> read)
		{
			ItemExtraInfoTable.Add(key, write);
			InterpretStringTable.Add(key, read);
		}
	}
}
