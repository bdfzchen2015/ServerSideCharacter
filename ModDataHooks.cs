using System;
using System.Collections.Generic;
using Terraria;

namespace ServerSideCharacter
{
	public class ModDataHooks
	{
		public static Dictionary<string, Func<Item, string>> ItemExtraInfoTable = new Dictionary<string, Func<Item, string>>();
		public static Dictionary<string, Action<string, Item>> InterpretStringTable = new Dictionary<string, Action<string, Item>>();
		public static Dictionary<string, Func<ServerPlayer, string>> PlayerExtraInfoTable = new Dictionary<string, Func<ServerPlayer, string>>();
		public static Dictionary<string, Action<string, ServerPlayer>> InterpretPlayerStringTable = new Dictionary<string, Action<string, ServerPlayer>>();

		public static void BuildItemDataHook(string key, Func<Item, string> write, Action<string, Item> read)
		{
			ItemExtraInfoTable.Add(key, write);
			InterpretStringTable.Add(key, read);
		}

		public static void BuildPlayerDataHook(string key, Func<ServerPlayer, string> write, Action<string, ServerPlayer> read)
		{
			PlayerExtraInfoTable.Add(key, write);
			InterpretPlayerStringTable.Add(key, read);
		}
	}
}
