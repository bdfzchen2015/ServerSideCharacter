using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace ServerSideCharacter.Buffs
{
    public class Locked : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffName[Type] = "Locked";
            Main.buffTip[Type] = "You are locked by the server";
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
			var modPlayer = player.GetModPlayer<MPlayer>(mod);
			modPlayer.Locked = true;
        }
    }
}