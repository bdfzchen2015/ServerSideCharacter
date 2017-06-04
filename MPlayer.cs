using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using System;

namespace ServerSideCharacter
{
	public class MPlayer : ModPlayer
	{
		public int playerCounter = 0;

		public bool Locked = false;

		public bool GodMode = false;

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
				//player.controlInv = false; // With this the users will not be abble to exit the server without login first (Exept by pressing ALT + F4). This is not a good thing
				player.gravDir = 0f;
				player.position = player.oldPosition;
			}
		}

		public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			return !GodMode;
		}

		public override void PreUpdate()
		{
			if (GodMode)
			{
				player.statLife = player.statLifeMax2;
			}
		}

		public override void OnEnterWorld(Player player)
		{
			if (Main.netMode == 1)
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
