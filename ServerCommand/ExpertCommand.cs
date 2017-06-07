using Terraria.ModLoader;

namespace ServerSideCharacter.ServerCommand
{
	public class ExpertCommand : ModCommand
	{
		public override string Command
		{
			get { return "expert"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Toggle world's expert mode"; }
		}

		public override string Usage
		{
			get { return "/expert"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendToggleExpert();
		}
	}
}
