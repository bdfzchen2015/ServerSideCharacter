using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml;
using Terraria.IO;
using Terraria.Localization;
using ServerSideCharacter.XMLHelper;

namespace ServerSideCharacter
{
    public class ServerPlayer
    {
        public string Name { get; set; }

        public string Password { get; set; }

        public string Hash { get; set; }

        public int LifeMax { get; set; }

        public int StatLife { get; set; }

        public int ManaMax { get; set; }

        public int StatMana { get; set; }

        public Item[] inventroy = new Item[59];

        public Item[] armor = new Item[20];

        public Item[] dye = new Item[10];

        public Item[] miscEquips = new Item[5];

        public Item[] miscDye = new Item[5];

        public ServerPlayer()
        {
            for(int i = 0; i < inventroy.Length; i++)
            {
                inventroy[i] = new Item();
            }
            for (int i = 0; i < armor.Length; i++)
            {
                armor[i] = new Item();
            }
            for (int i = 0; i < dye.Length; i++)
            {
                dye[i] = new Item();
            }
            for (int i = 0; i < miscEquips.Length; i++)
            {
                miscEquips[i] = new Item();
            }
            for (int i = 0; i < miscDye.Length; i++)
            {
                miscDye[i] = new Item();
            }
        }

        public static string GenHashCode(string name)
        {
            long hash = name.GetHashCode();
            hash = DateTime.Now.Millisecond + hash * 2333;
            short res = (short)(hash % 65536);
            return Convert.ToString(res, 16);
        }
    }
}
