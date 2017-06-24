#define DEBUGMODE

using ServerSideCharacter.Region;
using ServerSideCharacter.ServerCommand;
using System;
using System.Linq;
using System.Threading;
using Terraria.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter
{
	public class MWorld : ModWorld
	{
		public static bool ServerStarted = false;


		public override void PostUpdate()
		{
			if (Main.netMode == 2)
			{
				try
				{
					ServerStarted = true;
					for (int i = 0; i < 255; i++)
					{
						if (Main.player[i].active)
						{
							ServerPlayer player = ServerSideCharacter.XmlData.Data[Main.player[i].name];
							player.CopyFrom(Main.player[i]);
						}
					}
					if (Main.time % 180 < 1)
					{
						lock(ServerSideCharacter.XmlData)
						{
							foreach (var player in ServerSideCharacter.XmlData.Data)
							{
								if (player.Value.PrototypePlayer != null)
								{
									int playerID = player.Value.PrototypePlayer.whoAmI;
									if (!player.Value.HasPassword)
									{
										player.Value.ApplyLockBuffs();
										NetMessage.SendChatMessageToClient(NetworkText.FromLiteral("Welcome! You are new to here. Please use /register <password> to register an account!"), new Color(255, 255, 30, 30), playerID);
									}
									if (player.Value.HasPassword && !player.Value.IsLogin)
									{
										player.Value.ApplyLockBuffs();
										NetMessage.SendChatMessageToClient(NetworkText.FromLiteral("Welcome! You have already created an account. Please type /login <password> to login!"), new Color(255, 255, 30, 30), playerID);
									}
								}
							}
						}
					}
					if (Main.time % 3600 < 1)
					{
						ThreadPool.QueueUserWorkItem(Do_Save);
					}
					foreach (var player in Main.player.Where(p => p.active))
					{
						if (player.GetServerPlayer().EnteredRegion == null)
						{
							var serverPlayer = player.GetServerPlayer();
							RegionInfo region;
							if (serverPlayer.InAnyRegion(out region))
							{
								serverPlayer.EnteredRegion = region;
								serverPlayer.SendInfo(region.WelcomeInfo());
							}
						}
						else if (player.GetServerPlayer().EnteredRegion != null)
						{
							var serverPlayer = player.GetServerPlayer();
							RegionInfo region;
							if (!serverPlayer.InAnyRegion(out region))
							{
								serverPlayer.SendInfo(serverPlayer.EnteredRegion.LeaveInfo());
								serverPlayer.EnteredRegion = null;
							}
						}
					}
				}
				catch (Exception ex)
				{
					CommandBoardcast.ConsoleError(ex);
					WorldFile.saveWorld();
					Netplay.disconnect = true;
					Terraria.Social.SocialAPI.Shutdown();
				}
			}

		}

		private void Do_Save(object state)
		{
			foreach (var player in ServerSideCharacter.XmlData.Data)
			{
				try
				{
					ServerSideCharacter.MainWriter.SavePlayer(player.Value);
				}
				catch (Exception ex)
				{
					CommandBoardcast.ConsoleError(ex);
				}
			}
			ServerSideCharacter.Config.Save();
			CommandBoardcast.ConsoleSaveInfo();
		}


	}
}
