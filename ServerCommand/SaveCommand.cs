using Terraria.ModLoader;
using Terraria;

namespace ServerSideCharacter.ServerCommand
{
	public class SaveCommand : ModCommand
	{
		public override string Command
		{
			get { return "save"; }
		}

		public override CommandType Type
		{
			get { return CommandType.Chat; }
		}

		public override string Description
		{
			get { return "Save player's data"; }
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			MessageSender.SendRequestSave(Main.myPlayer);
			Main.NewText("Saved player's data");
		}
	}
}
