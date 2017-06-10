using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ServerSideCharacter.Buffs
{
	public class Locked : ModBuff
	{
		public override void SetDefaults()
		{
			DisplayName.SetDefault("Locked");
			Description.SetDefault("You are locked by the server");

			DisplayName.AddTranslation(GameCulture.Portuguese, "Bloqueado");
			Description.AddTranslation(GameCulture.Portuguese, "Você está bloqueado pelo servidor");

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