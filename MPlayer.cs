using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter
{
	public class MPlayer : ModPlayer
	{
		public int playerCounter = 0;

		public override void PreUpdate()
		{
			playerCounter++;
			if(playerCounter % 120 == 0)
			{
				NetSync.SendRequestSave(player.whoAmI);
				Main.NewText("Saving");
			}
		}
	}
}
