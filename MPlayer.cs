using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter
{
	public class MPlayer : ModPlayer
	{
		public int playerCounter = 0;

		public bool Locked = false;

		public override void ResetEffects()
		{
			Locked = false;
		}

		public override void SetControls()
		{
			if (Locked)
			{
				player.controlJump = false;
				player.controlDown = false;
				player.controlLeft = false;
				player.controlRight = false;
				player.controlUp = false;
				player.controlUseItem = false;
				player.controlUseTile = false;
				player.controlThrow = false;
				player.controlHook = false;
				player.controlMount = false;
				player.controlInv = false;
				player.gravDir = 0f;
				player.position = player.oldPosition;
			}
		}

		public override void OnEnterWorld(Player player)
		{
			if(Main.netMode == 1)
			{
				player.AddBuff(mod.BuffType("Locked"), 180);
			}
		}

		public override void PostUpdate()
		{

			playerCounter++;
		}
	}
}
