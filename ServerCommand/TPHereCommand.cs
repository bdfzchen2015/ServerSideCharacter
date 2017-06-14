using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace ServerSideCharacter.ServerCommand
{
	public class TPHereCommand : ModCommand
	{
		public override string Command
		{
			get { return "tphere"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Teleport a player to your position"; }
		}

		public override string Usage
		{
			get { return "/tphere <player id|player name>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			int who = Utils.TryGetPlayerID(args[0]);
			if (who == -1)
			{
				Main.NewText("Player not found", Color.Red);
				return;
			}
			ModPacket pack = ServerSideCharacter.Instance.GetPacket();
			pack.Write((int)SSCMessageType.TPHereCommand);
			pack.Write((byte)Main.myPlayer);
			pack.Write((byte)who);
			pack.Send();
		}
	}
}
