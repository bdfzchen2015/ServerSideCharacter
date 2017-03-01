using System;
using Terraria;
using Terraria.ID;

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

		public void CopyFrom(Player player)
		{
			this.LifeMax = player.statLifeMax;
			this.StatLife = player.statLife;
			this.StatMana = player.statMana;
			this.ManaMax = player.statManaMax;
			player.inventory.CopyTo(this.inventroy, 0);
			player.armor.CopyTo(this.armor, 0);
			player.dye.CopyTo(this.dye, 0);
			player.miscEquips.CopyTo(this.miscEquips, 0);
			player.miscDyes.CopyTo(this.miscDye, 0);
			this.bank = (Chest)player.bank.Clone();
			this.bank2 = (Chest)player.bank2.Clone();
			this.bank3 = (Chest)player.bank3.Clone();
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
			player.inventroy[0].SetDefaults(ItemID.TerraBlade);
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
