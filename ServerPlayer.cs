using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ServerSideCharacter.GroupManage;
using ServerSideCharacter.Region;
using Terraria;
using Terraria.ID;

namespace ServerSideCharacter
{
	public class ServerPlayer
	{

		
		public bool HasPassword { get; set; }

		public bool IsLogin { get; set; }

		public string Name { get; set; }

		public string Password { get; set; }

		public string Hash { get; set; }

		public Group PermissionGroup { get; set; }

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

		public Player prototypePlayer { get; set; }
        
        public RegionInfo enteredRegion { get; set; }

		public List<RegionInfo> ownedregion = new List<RegionInfo>();

		public static List<Item> StartUpItems = new List<Item>();


		private void SetupPlayer()
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
		public ServerPlayer()
		{
			SetupPlayer();
		}

		public ServerPlayer(Player player)
		{
			SetupPlayer();
			prototypePlayer = player;
		}

		public void CopyFrom(Player player)
		{
			LifeMax = player.statLifeMax;
			StatLife = player.statLife;
			StatMana = player.statMana;
			ManaMax = player.statManaMax;
			player.inventory.CopyTo(inventroy, 0);
			player.armor.CopyTo(armor, 0);
			player.dye.CopyTo(dye, 0);
			player.miscEquips.CopyTo(miscEquips, 0);
			player.miscDyes.CopyTo(miscDye, 0);
			bank = (Chest)player.bank.Clone();
			bank2 = (Chest)player.bank2.Clone();
			bank3 = (Chest)player.bank3.Clone();
		}


		public void ApplyLockBuffs(int time = 180)
		{
			prototypePlayer.AddBuff(ServerSideCharacter.Instance.BuffType("Locked"), time * 2, false);
			prototypePlayer.AddBuff(BuffID.Frozen, time, false);
			NetMessage.SendData(MessageID.AddPlayerBuff, prototypePlayer.whoAmI, -1,
				"", prototypePlayer.whoAmI,
				ServerSideCharacter.Instance.BuffType("Locked"), time * 2, 0f, 0, 0, 0);
			NetMessage.SendData(MessageID.AddPlayerBuff, prototypePlayer.whoAmI, -1,
				"", prototypePlayer.whoAmI,
				BuffID.Frozen, time, 0f, 0, 0, 0);
		}

        public void SendSuccessInfo(string msg)
        {
            NetMessage.SendData(MessageID.ChatText, prototypePlayer.whoAmI, -1,
                            msg,
                            255, 50, 255, 50);
        }
        public void SendInfo(string msg)
        {
            NetMessage.SendData(MessageID.ChatText, prototypePlayer.whoAmI, -1,
                            msg,
                            255, 255, 255, 0);
        }
        public void SendErrorInfo(string msg)
        {
            NetMessage.SendData(MessageID.ChatText, prototypePlayer.whoAmI, -1,
                            msg,
                            255, 255, 20, 0);
        }

        public static string GenHashCode(string name)
		{
			long hash = name.GetHashCode();
			hash += DateTime.Now.ToLongTimeString().GetHashCode() * 233;
			short res = (short)(hash % 65536);
			return Convert.ToString(res, 16);
		}

		public static ServerPlayer CreateNewPlayer(Player p)
		{
			ServerPlayer player = new ServerPlayer(p);
			int i = 0;
			foreach(var item in StartUpItems)
			{
				player.inventroy[i++] = item;
			}
			player.Name = p.name;
			player.Hash = GenHashCode(p.name);
			player.HasPassword = false;
			player.PermissionGroup = GroupType.Groups["default"];
			player.IsLogin = false;
			player.Password = "";
			player.LifeMax = 100;
			player.StatLife = 100;
			player.ManaMax = 20;
			player.StatMana = 20;
			return player;
		}

		public static ServerPlayer FindPlayer(string hash)
		{
			foreach (var pair in ServerSideCharacter.XmlData.Data)
			{
				if (pair.Value.Hash == hash)
				{
					return pair.Value;
				}
			}
			throw new Exception("Cannot find the player!");
		}

		public bool InAnyRegion(out RegionInfo region)
		{
			foreach (var reg in ServerSideCharacter.RegionManager.ServerRegions)
			{
				Rectangle worldArea = new Rectangle(reg.Area.X * 16, reg.Area.Y * 16,
					reg.Area.Width * 16, reg.Area.Height * 16);
				if (worldArea.Intersects(prototypePlayer.Hitbox))
				{
					region = reg;
					return true;
				}
			}
			region = null;
			return false;
		}
	}
}
