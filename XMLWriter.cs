using ServerSideCharacter.XMLHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

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
			for (int i = 0; i < 59; i++)
			{
				//TODO: Mod Item check

				var node1 = (XmlElement)NodeHelper.CreateNode(XMLDoc, playerNode, "slot_" + i, 
					player.inventroy[i].type.ToString());
				node1.SetAttribute("prefix", player.inventroy[i].prefix.ToString());
				node1.SetAttribute("stack", player.inventroy[i].stack.ToString());

				//TODO: Additional mod item info
			}
			for (int i = 0; i < 20; i++)
			{
				//TODO: Mod Item check
				var node1 = (XmlElement)NodeHelper.CreateNode(XMLDoc, playerNode, "armor_" + i, 
					player.armor[i].type.ToString());
				node1.SetAttribute("prefix", player.armor[i].prefix.ToString());
			}
			for (int i = 0; i < 10; i++)
			{
				NodeHelper.CreateNode(XMLDoc, playerNode, "dye_" + i, player.dye[i].type.ToString());
			}
			for (int i = 0; i < 5; i++)
			{
				NodeHelper.CreateNode(XMLDoc, playerNode, "miscEquip_" + i, player.miscEquips[i].type.ToString());
			}
			for (int i = 0; i < 5; i++)
			{
				NodeHelper.CreateNode(XMLDoc, playerNode, "miscDye_" + i, player.miscDye[i].type.ToString());
			}
			for(int i = 0; i < player.bank.item.Length; i++)
			{
				var node1 = (XmlElement)NodeHelper.CreateNode(XMLDoc, playerNode, "bank_" + i, player.bank.item[i].type.ToString());
				node1.SetAttribute("prefix", player.bank.item[i].prefix.ToString());
				node1.SetAttribute("stack", player.bank.item[i].stack.ToString());
			}
			for (int i = 0; i < player.bank2.item.Length; i++)
			{
				var node1 = (XmlElement)NodeHelper.CreateNode(XMLDoc, playerNode, "bank2_" + i, player.bank2.item[i].type.ToString());
				node1.SetAttribute("prefix", player.bank2.item[i].prefix.ToString());
				node1.SetAttribute("stack", player.bank2.item[i].stack.ToString());
			}
			for (int i = 0; i < player.bank3.item.Length; i++)
			{
				var node1 = (XmlElement)NodeHelper.CreateNode(XMLDoc, playerNode, "bank3_" + i, player.bank3.item[i].type.ToString());
				node1.SetAttribute("prefix", player.bank2.item[i].prefix.ToString());
				node1.SetAttribute("stack", player.bank2.item[i].stack.ToString());
			}
			PlayerRoot.AppendChild(playerNode);


			using (XmlTextWriter xtw = new XmlTextWriter(FilePath, Encoding.UTF8))
			{
				xtw.Formatting = Formatting.None;
				XMLDoc.Save(xtw);
			}
		}
	}
}
