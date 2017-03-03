using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;

namespace ServerSideCharacter.Plugin
{
	public abstract class ModExtraIO
	{
		public abstract string GetWriteString(Item item);

		public abstract void SetItemFromInput(string input, Item item);
	}
}
