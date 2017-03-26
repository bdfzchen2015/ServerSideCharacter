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
	public class ServerConfig
	{
		public List<Item> StartUpItems;

		public bool ConfigExist
		{
			get
			{
				return _xmlExist;
			}
		}

		private XmlDocument _document;

		private XmlNode _mainNode;

		private bool _xmlExist;



		public ServerConfig()
		{
			_document = new XmlDocument();
			StartUpItems = new List<Item>();
			_xmlExist = File.Exists("SSC/config.xml");
			SetNodes();
		}

		private void SetNodes()
		{
			if (!ConfigExist)
			{
				_mainNode = _document.CreateXmlDeclaration("1.0", "utf-8", "");
				_document.AppendChild(_mainNode);
			}
			else
			{
				XmlReaderSettings settings = new XmlReaderSettings()
				{
					IgnoreComments = true                         //忽略文档里面的注释
				};
				_document = new XmlDocument();
				XmlReader reader = XmlReader.Create("SSC/config.xml", settings);
				_document.Load(reader);
			}
		}

		private void SetUpStartInv()
		{
			if (!ConfigExist)
			{
				XmlNode root = _document.CreateElement("StartupInventory");
				_document.AppendChild(root);
				NodeHelper.CreateItem(_document, root, ItemID.ShadewoodSword, 82);
				NodeHelper.CreateItem(_document, root, ItemID.IronPickaxe, 83);
				NodeHelper.CreateItem(_document, root, ItemID.IronAxe, 81);
				AddToStartInv(ItemID.ShadewoodSword, 82);
				AddToStartInv(ItemID.IronPickaxe, 83);
				AddToStartInv(ItemID.IronAxe, 81);
				if (ModLoader.LoadedMods.Any(mod => mod.Name == "ThoriumMod"))
				{
					var thorium = ModLoader.LoadedMods.Where(mod => mod.Name == "ThoriumMod");
					AddToStartInv(thorium.First().ItemType("FamilyHeirloom"));
				}
			}
			else
			{
				var xn = _document.SelectSingleNode("StartupInventory");
				if (xn != null)
				{
					var list = xn.ChildNodes;
					var xmlNode = list.Item(0);
					if (xmlNode != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							XmlElement xe = (XmlElement) list.Item(i);
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
		}

		public void CreateConfig()
		{
			SetUpStartInv();
			if (!ConfigExist)
				_document.Save("SSC/config.xml");
		}


		private void AddToStartInv(int type, int prefex = 0)
		{
			Item item1 = new Item();
			item1.SetDefaults(type);
			item1.Prefix(prefex);
			StartUpItems.Add(item1);
		}
	}
}
