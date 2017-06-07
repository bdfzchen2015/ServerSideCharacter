using Terraria;

namespace ServerSideCharacter.Plugin
{
	public abstract class ModExtraIO
	{
		public abstract string GetWriteString(Item item);

		public abstract void SetItemFromInput(string input, Item item);
	}
}
