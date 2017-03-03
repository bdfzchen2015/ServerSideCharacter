using ServerSideCharacter.XMLHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Terraria;

namespace ServerSideCharacter
{
	public class XMLWriter
	{
		public string FilePath { get; set; }

		public XmlDocument XMLDoc;

		public XmlNode PlayerRoot;

		public XMLWriter()
		{

		}

		public XMLWriter(string path)
		{
			FilePath = path;
			if (!File.Exists(path))
			{
				Create();
			}
			else
			{
				XMLDoc = new XmlDocument();
				XMLDoc.Load(path);
				PlayerRoot = XMLDoc.SelectSingleNode("Players");
			}
		}

		public void Create()
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
			xmlDoc.AppendChild(node);
			//创建根节点    
			XmlNode root = xmlDoc.CreateElement("Players");
			xmlDoc.AppendChild(root);
			XMLDoc = xmlDoc;
			PlayerRoot = root;
		}

		private XmlElement WriteItemInfo(XmlNodeList list, int i, ref int j, ref Item[] slots)
		{
			Item item = slots[i];
			XmlElement node1;
			if (item.type < Main.maxItemTypes)
			{
				node1 = (XmlElement)WriteNext(list, ref j, item.type.ToString());
			}
			else
			{
				node1 = (XmlElement)WriteNext(list, ref j, "$" + item.modItem.GetType().FullName);
			}
			foreach (var pair in ModDataHooks.ItemExtraInfoTable)
			{
				node1.SetAttribute(pair.Key, pair.Value(slots[i]));
			}
			return node1;
		}

		private XmlElement CreateItemInfo(XmlNode parent, int i, ref Item[] slots, string name)
		{
			Item item = slots[i];
			XmlElement node1;
			if (item.type < Main.maxItemTypes)
			{
				node1 = (XmlElement)NodeHelper.CreateNode(XMLDoc, parent, name + "_" + i,
					item.type.ToString());
			}
			else
			{
				node1 = (XmlElement)NodeHelper.CreateNode(XMLDoc, parent, name + "_" + i,
					"$" + item.modItem.GetType().FullName);
			}
			foreach (var pair in ModDataHooks.ItemExtraInfoTable)
			{
				node1.SetAttribute(pair.Key, pair.Value(slots[i]));
			}
			return node1;
		}

		private XmlNode WriteNext(XmlNodeList list, ref int i, string toWrite)
		{
			var node = list.Item(i);
			list.Item(i++).InnerText = toWrite;
			return node;
		}

		public void SavePlayer(ServerPlayer player)
		{
			XmlElement targetNode = null;
			foreach(var node in PlayerRoot.ChildNodes)
			{
				XmlElement element = node as XmlElement;
				if (element.GetAttribute("name").Equals(player.Name))
				{
					targetNode = element;
					break;
				}
			}
			if (targetNode == null)
			{
				Console.WriteLine("Creating new Account");
				Write(player);
			}
			else
			{
				var list = targetNode.ChildNodes;
				int j = 0;
				WriteNext(list, ref j, player.HasPassword.ToString());
				WriteNext(list, ref j, player.Password);
				WriteNext(list, ref j, player.LifeMax.ToString());
				WriteNext(list, ref j, player.StatLife.ToString());
				WriteNext(list, ref j, player.ManaMax.ToString());
				WriteNext(list, ref j, player.StatMana.ToString());
				for (int i = 0; i < player.inventroy.Length; i++)
				{
					//TODO: Mod Item check
					var node1 = WriteItemInfo(list, i, ref j, ref player.inventroy);
					//TODO: Additional mod item info
				}
				for (int i = 0; i < player.armor.Length; i++)
				{
					//TODO: Mod Item check
					var node1 = WriteItemInfo(list, i, ref j, ref player.armor);
				}
				for (int i = 0; i < player.dye.Length; i++)
				{
					var node1 = WriteItemInfo(list, i, ref j, ref player.dye);
				}
				for (int i = 0; i < player.miscEquips.Length; i++)
				{
					var node1 = WriteItemInfo(list, i, ref j, ref player.miscEquips);
				}
				for (int i = 0; i < player.miscDye.Length; i++)
				{
					var node1 = WriteItemInfo(list, i, ref j, ref player.miscDye);
				}
				for (int i = 0; i < player.bank.item.Length; i++)
				{
					var node1 = WriteItemInfo(list, i, ref j, ref player.bank.item);
				}
				for (int i = 0; i < player.bank2.item.Length; i++)
				{
					var node1 = WriteItemInfo(list, i, ref j, ref player.bank2.item);
				}
				for (int i = 0; i < player.bank3.item.Length; i++)
				{
					var node1 = WriteItemInfo(list, i, ref j, ref player.bank3.item);
				}
				using (XmlTextWriter xtw = new XmlTextWriter(FilePath, Encoding.UTF8))
				{
					xtw.Formatting = Formatting.Indented;
					XMLDoc.Save(xtw);
				}
			}
			Console.WriteLine("XML Saved!");
		}


		public void Write(ServerPlayer player)
		{

			XmlNode playerNode = XMLDoc.CreateNode(XmlNodeType.Element, "Player", null);
			XmlElement element = playerNode as XmlElement;
			element.SetAttribute("name", player.Name);
			element.SetAttribute("hash", ServerPlayer.GenHashCode(player.Name));
			NodeHelper.CreateNode(XMLDoc, playerNode, "haspwd", player.HasPassword.ToString());
			NodeHelper.CreateNode(XMLDoc, playerNode, "password", player.Password);
			NodeHelper.CreateNode(XMLDoc, playerNode, "lifeMax", player.LifeMax.ToString());
			NodeHelper.CreateNode(XMLDoc, playerNode, "statlife", player.StatLife.ToString());
			NodeHelper.CreateNode(XMLDoc, playerNode, "manaMax", player.ManaMax.ToString());
			NodeHelper.CreateNode(XMLDoc, playerNode, "statmana", player.StatMana.ToString());
			for (int i = 0; i < player.inventroy.Length; i++)
			{
				//TODO: Mod Item check
				var node1 = CreateItemInfo(playerNode, i, ref player.inventroy, "slot");
				//TODO: Additional mod item info
			}
			for (int i = 0; i < player.armor.Length; i++)
			{
				//TODO: Mod Item check
				var node1 = CreateItemInfo(playerNode, i, ref player.armor, "armor");
			}
			for (int i = 0; i < player.dye.Length; i++)
			{
				var node1 = CreateItemInfo(playerNode, i, ref player.dye, "dye");
			}
			for (int i = 0; i < player.miscEquips.Length; i++)
			{
				var node1 = CreateItemInfo(playerNode, i, ref player.miscEquips, "miscEquips");
			}
			for (int i = 0; i < player.miscDye.Length; i++)
			{
				var node1 = CreateItemInfo(playerNode, i, ref player.miscDye, "miscDye");
			}
			for(int i = 0; i < player.bank.item.Length; i++)
			{
				var node1 = CreateItemInfo(playerNode, i, ref player.bank.item, "bank");
			}
			for (int i = 0; i < player.bank2.item.Length; i++)
			{
				var node1 = CreateItemInfo(playerNode, i, ref player.bank2.item, "bank2");
			}
			for (int i = 0; i < player.bank3.item.Length; i++)
			{
				var node1 = CreateItemInfo(playerNode, i, ref player.bank3.item, "bank3");
			}
			PlayerRoot.AppendChild(playerNode);


			using (XmlTextWriter xtw = new XmlTextWriter(FilePath, Encoding.UTF8))
			{
				xtw.Formatting = Formatting.Indented;
				XMLDoc.Save(xtw);
			}
		}
	}
}
