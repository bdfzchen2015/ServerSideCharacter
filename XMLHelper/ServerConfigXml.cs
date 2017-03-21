using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ServerSideCharacter.XMLHelper
{
	public class ServerConfigXml
	{
		public static void SetUpStartInv()
		{

			if (!File.Exists("SSC/config.xml"))
			{
				XmlDocument xmlDoc = new XmlDocument();
				XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
				xmlDoc.AppendChild(node);
				XmlNode root = xmlDoc.CreateElement("StartupInventory");
				xmlDoc.AppendChild(root);
				NodeHelper.CreateNode(xmlDoc, root, "count", "3");
				NodeHelper.CreateItem(xmlDoc, root, ItemID.ShadewoodSword, 82);
				NodeHelper.CreateItem(xmlDoc, root, ItemID.IronPickaxe, 83);
				NodeHelper.CreateItem(xmlDoc, root, ItemID.IronAxe, 81);
				xmlDoc.Save("SSC/config.xml");
				AddToStartInv(ItemID.ShadewoodSword, 82);
				AddToStartInv(ItemID.IronPickaxe, 83);
				AddToStartInv(ItemID.IronAxe, 81);
			}
			else
			{
				XmlReaderSettings settings = new XmlReaderSettings()
				{
					IgnoreComments = true                         //忽略文档里面的注释
				};
				XmlDocument xmlDoc = new XmlDocument();
				XmlReader reader = XmlReader.Create("SSC/config.xml", settings);
				xmlDoc.Load(reader);
				XmlNode xn = xmlDoc.SelectSingleNode("StartupInventory");
				if (xn != null)
				{
					var list = xn.ChildNodes;
					var xmlNode = list.Item(0);
					if (xmlNode != null)
					{
						int count = Convert.ToInt32(xmlNode.InnerText);
						for (int i = 0; i < count; i++)
						{
							XmlElement xe = (XmlElement) list.Item(i + 1);
							AddToStartInv(Convert.ToInt32(xe.InnerText), Convert.ToInt32(xe.GetAttribute("prefix")));
						}
					}
					else
					{
						throw new NullReferenceException("XMLNode is null");
					}
				}
				else
				{
					throw new NullReferenceException("XMLNode is null");
				}
			}

			if (ModLoader.LoadedMods.Any(mod => mod.Name == "ThoriumMod"))
			{
				var thorium = ModLoader.LoadedMods.Where(mod => mod.Name == "ThoriumMod");
				AddToStartInv(thorium.First().ItemType("FamilyHeirloom"));
			}
		}


		private static void AddToStartInv(int type, int prefex = 0)
		{
			Item item1 = new Item();
			item1.SetDefaults(type);
			item1.Prefix(prefex);
			ServerPlayer.StartUpItems.Add(item1);
		}
	}
}
