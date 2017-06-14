using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class TeleportCommand : ModCommand
	{
		public override string Command
		{
			get { return "tp"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Teleport to a player"; }
		}

		public override string Usage
		{
			get { return "/tp <player id|player name>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			args = Utils.ParseArgs(args);
			int who = Utils.TryGetPlayerID(args[0]);
			if (who == -1)
			{
				Main.NewText("Player not found", Color.Red);
				return;
			}
			MessageSender.SendTeleportCommand(Main.myPlayer, who);
		}
	}
}
