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

	}
}
