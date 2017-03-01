using System;
using Terraria;

namespace ServerSideCharacter
{
	public class ServerPlayer
	{

		//TODO: Write this in XML Doc
		public bool HasPassword { get; set; }

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

		public Chest bank = new Chest(true);

		public Chest bank2 = new Chest(true);

		public Chest bank3 = new Chest(true);


		public ServerPlayer()
		{
			for (int i = 0; i < inventroy.Length; i++)
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
			for (int i = 0; i < bank.item.Length; i++)
			{
				bank.item[i] = new Item();
			}
			for (int i = 0; i < bank2.item.Length; i++)
			{
				bank2.item[i] = new Item();
			}
			for (int i = 0; i < bank3.item.Length; i++)
			{
				bank3.item[i] = new Item();
			}
		}

		public static string GenHashCode(string name)
		{
			long hash = name.GetHashCode();
			hash += DateTime.Now.ToLongTimeString().GetHashCode() * 233;
			short res = (short)(hash % 65536);
			return Convert.ToString(res, 16);
		}

		public static ServerPlayer CreateNewPlayer(string name)
		{
			ServerPlayer player = new ServerPlayer();
			player.Name = name;
			player.Hash = GenHashCode(name);
			player.HasPassword = false;
			player.Password = "";
			player.LifeMax = 100;
			player.StatLife = 100;
			player.ManaMax = 20;
			player.StatMana = 20;
			return player;
		}
	}
}
