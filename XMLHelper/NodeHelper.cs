using System.Xml;

namespace ServerSideCharacter.XMLHelper
{
	public class NodeHelper
	{
		public static XmlNode CreateNode(XmlDocument xmlDoc, XmlNode parentNode, string name, string value)
		{
			XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);
			node.InnerText = value;
			parentNode.AppendChild(node);
			return node;
		}

		public static XmlNode CreateItem(XmlDocument xmlDoc, XmlNode parentNode, int type, int prefix)
		{
			XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, "item", null);
			node.InnerText = type.ToString();
			((XmlElement)node).SetAttribute("prefix", prefix.ToString());
			parentNode.AppendChild(node);
			return node;
		}
	}
}
