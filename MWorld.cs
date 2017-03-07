#define DEBUGMODE

using ServerSideCharacter.Region;
using ServerSideCharacter.ServerCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
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

			//Item g = new Item();
			//g.SetDefaults(100);
			//MessageBox.Show((g.modItem == null).ToString());

			//foreach (var othermod in ModLoader.LoadedMods)
			//{
			//	//string modName = othermod.FullName.Substring(0, item.FullName.IndexOf('.'));
			//		MessageBox.Show(othermod.Name);
			//}
			if (Main.netMode == 2)
			{
				ServerStarted = true;
				if (Main.time % 120 < 1)
				{
					for (int i = 0; i < 255; i++)
					{
						if (Main.player[i].active)
						{
							ServerPlayer player = ServerSideCharacter.XmlData.Data[Main.player[i].name];
							player.CopyFrom(Main.player[i]);
						}
					}
				}
				if(Main.time % 180 < 1)
				{
					foreach (var player in ServerSideCharacter.XmlData.Data)
					{
						if (player.Value.prototypePlayer != null)
						{
							int playerID = player.Value.prototypePlayer.whoAmI;
							if (!player.Value.HasPassword)
							{
								player.Value.ApplyLockBuffs();
								NetMessage.SendData(MessageID.ChatText, playerID, -1,
								"Welcome! You are new to here. Please use /register <password> to register an account!",
								255, 255, 30, 30);
								continue;
							}
							if (!player.Value.IsLogin)
							{
								player.Value.ApplyLockBuffs();
								NetMessage.SendData(MessageID.ChatText, playerID, -1,
								"Welcome! You have already created an account. Please type /login <password> to login!",
								255, 255, 30, 30);
							}
						}
					}
				}
				if (Main.time % 3600 < 1)
				{
					ThreadPool.QueueUserWorkItem(do_Save);
				}
				foreach(var player in Main.player)
				{
					if (player.active && player.GetServerPlayer().enteredRegion == null)
					{
						var serverPlayer = player.GetServerPlayer();
						RegionInfo region;
						if (serverPlayer.InAnyRegion(out region))
						{
							serverPlayer.enteredRegion = region;
							serverPlayer.SendInfo(region.WelcomeInfo());
						}
					}
					else if(player.GetServerPlayer().enteredRegion != null)
					{
						var serverPlayer = player.GetServerPlayer();
						RegionInfo region;
						if (!serverPlayer.InAnyRegion(out region))
						{
							serverPlayer.SendInfo(serverPlayer.enteredRegion.LeaveInfo());
							serverPlayer.enteredRegion = null;
						}
					}
				}
			}
		}

		private void do_Save(object state)
		{
			foreach (var player in ServerSideCharacter.XmlData.Data)
			{
				try
				{
					ServerSideCharacter.MainWriter.SavePlayer(player.Value);
				}
				catch (Exception ex)
				{
					CommandBoardcast.ShowError(ex);
				}
			}
			CommandBoardcast.ShowSaveInfo();
		}

	}
}
