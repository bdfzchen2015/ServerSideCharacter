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
        public override void SetDefaults()
        {
            item.name = "Temporary Summoner";
            item.toolTip = "Summons the King of Brain";
            item.height = 32;
            item.width = 32;
            item.maxStack = 30;
            item.rare = 11;
            item.expert = true;
            item.value = 0;
            item.useTime = 30;
            item.useAnimation = 30;
            item.useStyle = 4;
        }


        public override void AddRecipes()
        {
            ModRecipe re = new ModRecipe(mod);
            re.SetResult(this);
            re.AddRecipe();
        }
    }
}