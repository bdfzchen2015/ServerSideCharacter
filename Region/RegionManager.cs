using Microsoft.Xna.Framework;
using ServerSideCharacter.XMLHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Terraria;

namespace ServerSideCharacter.Region
{
	public class RegionManager
	{
		public List<RegionInfo> ServerRegions = new List<RegionInfo>();

		public void CreateNewRegion(Rectangle rect, string Name, ServerPlayer player)
		{
			RegionInfo playerRegion = new RegionInfo(Name, player, rect);
			ServerRegions.Add(playerRegion);
			player.ownedregion.Add(playerRegion);
		}

		public bool HasNameConflect(string name)
		{
			foreach(var region in ServerRegions)
			{
				if (name.Equals(region.Name))
				{
					return true;
				}
			}
			return false;
		}

		public bool CheckPlayerRegionMax(ServerPlayer player)
		{
			return ServerRegions.Count(info => info.Owner.Equals(player)) < 3;
		}

		public bool CheckRegionConflict(Rectangle rect)
		{
			foreach(var region in ServerRegions)
			{
				if (region.Area.Intersects(rect))
				{
					return false;
				}
			}
			return true;
		}

		public void ReadRegionInfo()
		{
			if (!File.Exists("SSC/regions.xml"))
			{
				XmlDocument xmlDoc = new XmlDocument();
				XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
				xmlDoc.AppendChild(node);
				XmlNode root = xmlDoc.CreateElement("Regions");
				xmlDoc.AppendChild(root);
				xmlDoc.Save("SSC/regions.xml");
			}
			else
			{
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.IgnoreComments = true;                         //忽略文档里面的注释
				XmlDocument xmlDoc = new XmlDocument();
				XmlReader reader = XmlReader.Create("SSC/regions.xml", settings);
				xmlDoc.Load(reader);
				XmlNode xn = xmlDoc.SelectSingleNode("Regions");
				var list = xn.ChildNodes;
				foreach(var node in list)
				{
					XmlElement regionData = (XmlElement)node;

					string hash = regionData.GetAttribute("hash");
					var info = regionData.ChildNodes;
					Rectangle area = new Rectangle();
					string name = info.Item(0).InnerText;
					area.X = Convert.ToInt32(info.Item(1).InnerText);
					area.Y = Convert.ToInt32(info.Item(2).InnerText);
					area.Width = Convert.ToInt32(info.Item(3).InnerText);
					area.Height = Convert.ToInt32(info.Item(4).InnerText);
					ServerPlayer player = ServerPlayer.FindPlayer(hash);
					ServerSideCharacter.regionManager.CreateNewRegion(area, name, player); 
				}
			}
		}

		public void WriteRegionInfo()
		{
			XmlDocument xmlDoc = new XmlDocument();
			XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
			xmlDoc.AppendChild(node);
			XmlNode root = xmlDoc.CreateElement("Regions");
			xmlDoc.AppendChild(root);
			foreach (var region in ServerRegions)
			{
				XmlElement xe = (XmlElement)xmlDoc.CreateNode(XmlNodeType.Element, "Region", null);
				xe.SetAttribute("hash", region.Owner.Hash);
				NodeHelper.CreateNode(xmlDoc, xe, "Name", region.Name);
				NodeHelper.CreateNode(xmlDoc, xe, "X", region.Area.X.ToString());
				NodeHelper.CreateNode(xmlDoc, xe, "Y", region.Area.Y.ToString());
				NodeHelper.CreateNode(xmlDoc, xe, "W", region.Area.Width.ToString());
				NodeHelper.CreateNode(xmlDoc, xe, "H", region.Area.Height.ToString());
				root.AppendChild(xe);
			}

			xmlDoc.Save("SSC/regions.xml");

		}

		public bool CheckRegion(int X, int Y, ServerPlayer player)
		{
			Vector2 TilePos = new Vector2(X, Y);
			foreach(var regions in ServerRegions)
			{
				if(regions.Area.Contains(X, Y) && !regions.Owner.Equals(player) && !regions.SharedOwner.Contains(player))
				{
					return true;
				}
			}
			return false;
		}

        public bool CheckRegionSize(ServerPlayer player, Rectangle area)
        {
            return player.PermissionGroup.isSuperAdmin() ? true : (area.Width < 35 && area.Height < 35);
        }

		internal bool ValidRegion(ServerPlayer player, string name, Rectangle area)
		{
			return !HasNameConflect(name) && ServerRegions.Count < 512 
				&& CheckPlayerRegionMax(player) && CheckRegionConflict(area) 
                && CheckRegionSize(player, area);
		}
	}
}
