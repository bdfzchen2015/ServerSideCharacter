using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class AuthCommand : ModCommand
	{
		public override string Command
		{
			get { return "auth"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Authorize player as admin"; }
		}

		public override string Usage
		{
			get { return "/auth <code>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendAuthRequest(Main.myPlayer, args[0]);
		}
	}
}
