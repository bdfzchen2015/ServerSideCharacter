using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace ServerSideCharacter
{
	public class NetSync
	{
		private Player player;
		private int to;
		private int from;

		public NetSync(int playerID, int to, int from)
		{
			player = Main.player[playerID];
			this.to = to;
			this.from = from;
		}

		public void SyncPlayerData()
		{

		}

		public static void SyncPlayerHealth(int plr, int to, int from)
		{
			string name = Main.player[plr].name;
			ServerPlayer player = ServerSideCharacter.xmlData.Data[name];
			Main.player[plr].statLife = player.StatLife;
			Main.player[plr].statLifeMax = player.LifeMax;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.SyncPlayerHealth);
			p.Write((byte)plr);
			p.Write(player.StatLife);
			p.Write(player.LifeMax);
			p.Send(to, from);
		}
		public static void SyncPlayerMana(int plr, int to, int from)
		{
			string name = Main.player[plr].name;
			ServerPlayer player = ServerSideCharacter.xmlData.Data[name];
			Main.player[plr].statMana = player.StatMana;
			Main.player[plr].statManaMax = player.ManaMax;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.SyncPlayerMana);
			p.Write((byte)plr);
			p.Write(player.StatMana);
			p.Write(player.ManaMax);
			p.Send(to, from);
		}
		public static void SyncPlayerBanks(int plr, int to, int from)
		{
			string name = Main.player[plr].name;
			ServerPlayer player = ServerSideCharacter.xmlData.Data[name];
			Main.player[plr].bank = (Chest)player.bank.Clone();
			Main.player[plr].bank2 = (Chest)player.bank2.Clone();
			Main.player[plr].bank3 = (Chest)player.bank3.Clone();
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.SyncPlayerBank);
			p.Write((byte)plr);
			for (int k = 0; k < player.bank.item.Length; k++)
			{
				p.Write(player.bank.item[k].type);
				p.Write((short)player.bank.item[k].prefix);
				p.Write((short)player.bank.item[k].stack);
			}
			for (int k = 0; k < player.bank2.item.Length; k++)
			{
				p.Write(player.bank2.item[k].type);
				p.Write((short)player.bank2.item[k].prefix);
				p.Write((short)player.bank2.item[k].stack);
			}
			for (int k = 0; k < player.bank3.item.Length; k++)
			{
				p.Write(player.bank3.item[k].type);
				p.Write((short)player.bank3.item[k].prefix);
				p.Write((short)player.bank3.item[k].stack);
			}
			p.Send(to, from);
		}

		public static void SendTeleport(int plr, Vector2 pos)
		{
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.TeleportPalyer);
			p.WriteVector2(pos);
			p.Send(plr, -1);
		}

		public static void SendRequestSave(int plr)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.RequestSaveData);
			p.Write((byte)plr);
			p.Send();
		}

		public static void SendTimeSet(double time, bool day)
		{
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.SendTimeSet);
			p.Write(time);
			p.Write(day);
			p.Write(Main.sunModY);
			p.Write(Main.moonModY);
			p.Send();
		}

		public static void SendSetPassword(int plr, string password)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.RequestRegister);
			p.Write((byte)plr);
			p.Write(password);
			p.Send();
		}

		public static void SendLoginPassword(int plr, string password)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.SendLoginPassword);
			p.Write((byte)plr);
			p.Write(password);
			p.Send();
		}

		public static void SendKillCommand(int plr, int target)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.KillCommand);
			p.Write((byte)plr);
			p.Write((byte)target);
			p.Send();
		}

		public static void SendTimeCommand(int plr, bool set, int time, bool day)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.TimeCommand);
			p.Write((byte)plr);
			p.Write(set);
			p.Write(time);
			p.Write(day);
			p.Send();
		}

		public static void SendLockCommand(int plr, int target, int time)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.LockPlayer);
			p.Write((byte)plr);
			p.Write((byte)target);
			p.Write(time);
			p.Send();
		}

		public static void SendItemCommand(int type)
		{
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.RequestItem);
			p.Write((byte)Main.myPlayer);
			p.Write(type);
			p.Send();
		}

		public static void SendTeleportCommand(int plr, int target)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.TPCommand);
			p.Write((byte)plr);
			p.Write((byte)target);
			p.Send();
		}

		public static void SendListCommand(int plr)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.ListCommand);
			p.Write((byte)plr);
			p.Send();
		}

		public static void SendHelpCommand(int plr)
		{
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.HelpCommand);
			p.Write((byte)plr);
			p.Send();
		}

		public static void SendButcherCommand(int plr)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.ButcherCommand);
			p.Write((byte)plr);
			p.Send();
		}

		public static void SendAuthRequest(int plr, string code)
		{
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.RequestAuth);
			p.Write((byte)plr);
			p.Write(code); 
			p.Send();
		}

		public static void SendSetGroup(int plr, string hash, string group)
		{
			string name = Main.player[plr].name;
			ModPacket p = ServerSideCharacter.instance.GetPacket();
			p.Write((int)SSCMessageType.RequestSetGroup);
			p.Write((byte)plr);
			p.Write(hash);
			p.Write(group);
			p.Send();
		}
	}
}
