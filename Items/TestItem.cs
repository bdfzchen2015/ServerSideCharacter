using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace ServerSideCharacter.Items
{
	public class TestItem : ModItem
	{
		public string FullName;

		public override void SetDefaults()
		{
			item.height = 32;
			item.width = 32;
			item.rare = 10;
			item.expert = true;
			item.value = 0;
			item.useTime = 30;
			item.useAnimation = 30;
			item.useStyle = 4;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Unloaded Item");
			DisplayName.AddTranslation(GameCulture.Portuguese, "Item Descarregado");
		}

		public void SetUp(string fullName)
		{
			string modName = fullName.Substring(0, fullName.IndexOf('.'));
			string itemName = fullName.Substring(fullName.LastIndexOf('.') + 1);
			Tooltip.SetDefault("Mod: " + modName + Environment.NewLine + "Name: " + itemName);
			Tooltip.AddTranslation(GameCulture.Portuguese, $"Mod: {modName}{Environment.NewLine}Nome: {itemName}");
			FullName = fullName;
		}
	}
}