using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter
{
    public class MPlayer : ModPlayer
    {
        public override void PreUpdate()
        {
            if (Main.time % 120 < 1)
                Main.NewText(Main.ServerSideCharacter.ToString());
        }
    }
}
