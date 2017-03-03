using System;
using Terraria;
using Terraria.ModLoader;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace ServerSideCharacter.Items
{
    public class TestItem : ModItem
    {
		public string FullName;

        public override void SetDefaults()
        {
            item.name = "Unloaded Item";
            item.height = 32;
            item.width = 32;
            item.rare = 10;
            item.expert = true;
            item.value = 0;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = 4;
        }

		public void SetUp(string fullName)
		{
			string modName = fullName.Substring(0, fullName.IndexOf('.'));
			string itemName = fullName.Substring(fullName.LastIndexOf('.') + 1);
			item.toolTip = "Mod: " + modName;
			item.toolTip2 = "Name: " + itemName;
			FullName = fullName;
		}
    }
}