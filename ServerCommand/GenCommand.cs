using Terraria.ModLoader;

namespace ServerSideCharacter.ServerCommand
{
	public class GenCommand : ModCommand
	{
		public override string Command
		{
			get { return "gen"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Regenerate world resources"; }
		}

		public override string Usage
		{
			get { return "/gen [-tree/-chest/-ore/-trap]"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			string type = args[0];
			GenerationType genType = GenerationType.Tree;
			switch (type)
			{
				case "-tree":
					{
						break;
					}
				default:
					break;
			}
			MessageSender.SendGeneration(genType);
		}
	}
}
