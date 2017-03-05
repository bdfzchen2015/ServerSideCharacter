using System;
using Terraria;
using Terraria.ModLoader;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace ServerSideCharacter.Items
{
    public class RegionItem : ModItem
    {
		public string FullName;

        public override void SetDefaults()
        {
            item.name = "Region Item";
            item.height = 32;
            item.width = 32;
            item.rare = 10;
            item.expert = true;
            item.value = 0;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = 4;
        }
		public override bool AltFunctionUse(Player player)
		{
			return true;
		}
		public override bool UseItem(Player player)
		{

			if (player.altFunctionUse != 2 && Main.mouseLeftRelease)
			{
				Vector2 tilePos = new Vector2(Player.tileTargetX, Player.tileTargetY);
				ServerSideCharacter.TilePos1 = tilePos;
				Main.NewText(string.Format("Selected tile positon 1 at ({0}, {1})", tilePos.X, tilePos.Y));
			}
			else if (player.altFunctionUse == 2 && Main.mouseRightRelease)
			{
				Vector2 tilePos = new Vector2(Player.tileTargetX, Player.tileTargetY);
				ServerSideCharacter.TilePos2 = tilePos;
				Main.NewText(string.Format("Selected tile positon 2 at ({0}, {1})", tilePos.X, tilePos.Y));
			}
			return true;
		}

	}
}