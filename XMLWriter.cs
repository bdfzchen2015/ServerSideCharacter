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
				var node1 = (XmlElement)NodeHelper.CreateNode(XMLDoc, playerNode, "slot_" + i, "0");
				node1.SetAttribute("prefix", "0");
				node1.SetAttribute("stack", "0");
			}
			for (int i = 59; i < 79; i++)
			{
				var node1 = (XmlElement)NodeHelper.CreateNode(XMLDoc, playerNode, "slot_" + i, "0");
				node1.SetAttribute("prefix", "0");
			}
			for (int i = 79; i < 89; i++)
			{
				NodeHelper.CreateNode(XMLDoc, playerNode, "slot_" + i, "0");
			}
			for (int i = 89; i < 94; i++)
			{
				NodeHelper.CreateNode(XMLDoc, playerNode, "slot_" + i, "0");
			}
			for (int i = 94; i < 99; i++)
			{
				NodeHelper.CreateNode(XMLDoc, playerNode, "slot_" + i, "0");
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
