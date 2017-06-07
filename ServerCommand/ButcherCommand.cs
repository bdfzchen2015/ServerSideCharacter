using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class ButcherCommand : ModCommand
	{
		public override string Command
		{
			get { return "butcher"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Butcher all enemies"; }
		}

		public override string Usage
		{
			get { return "/butcher"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendButcherCommand(Main.myPlayer);
		}
	}
}
