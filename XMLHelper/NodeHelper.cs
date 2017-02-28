using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
