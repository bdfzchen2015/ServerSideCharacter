using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ServerSideCharacter
{
	public class MWorld : ModWorld
	{
		public static bool ServerStarted = false;

		public override void PreUpdate()
		{
			if (Main.netMode == 2)
			{
				ServerStarted = true;
				if (Main.time % 180 < 1)
				{
					for (int i = 0; i < 255; i++)
					{
						if (Main.player[i].active)
						{
							ServerPlayer player = ServerSideCharacter.xmlData.Data[Main.player[i].name];
							player.CopyFrom(Main.player[i]);
						}
					}
					foreach (var player in ServerSideCharacter.xmlData.Data)
					{
						int playerID = player.Value.prototypePlayer.whoAmI;
						if (!player.Value.HasPassword)
						{
							player.Value.ApplyLockBuffs();
							NetMessage.SendData(MessageID.ChatText, playerID, -1,
							"Welcome! You are new to here. Please use /register <password> to register an account!",
							255, 255, 30, 30);
						}
						else if(!player.Value.IsLogin)
						{
							player.Value.ApplyLockBuffs();
							NetMessage.SendData(MessageID.ChatText, playerID, -1,
							"Welcome! You have already created an account. Please type /login <password> to login!",
							255, 255, 30, 30);
						}
					}
				}
				if (Main.time % 600 < 1)
				{
					foreach (var player in ServerSideCharacter.xmlData.Data)
					{
						try
						{
							ServerSideCharacter.MainWriter.SavePlayer(player.Value);
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex);
						}
						Console.WriteLine("Saved " + player.Key);
					}
				}
			}
		}
	}
}
