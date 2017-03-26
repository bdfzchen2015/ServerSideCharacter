using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class ResetCommand : ModCommand
	{
		public override string Command
		{
			get { return "reset"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Console; }
		}

		public override string Description
		{
			get { return "Reset player's password (Super Admin only)"; }
		}

		public override string Usage
		{
			get { return "/reset <hash>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			string hash = args[0];
			try
			{
				ServerPlayer player = ServerPlayer.FindPlayer(hash);
				player.HasPassword = false;
				player.IsLogin = false;
				player.SavePlayer();
				if (player.PrototypePlayer != null)
				{
					player.SendSuccessInfo("Your password has been reseted! Please register with another password");
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
