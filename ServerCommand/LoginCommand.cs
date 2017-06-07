using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class LoginCommand : ModCommand
	{
		public override string Command
		{
			get { return "login"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Login to the server"; }
		}

		public override string Usage
		{
			get { return "/login <your password>"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendLoginPassword(Main.myPlayer, args[0]);
		}
	}
}
