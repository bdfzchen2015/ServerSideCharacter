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

		public void CreateNewRegion(Rectangle rect, string name, ServerPlayer player)
		{
			lock (ServerRegions)
			{
				RegionInfo playerRegion = new RegionInfo(name, player, rect);
				ServerRegions.Add(playerRegion);
				player.ownedregion.Add(playerRegion);
			}
		}

		public bool RemoveRegionWithName(string name)
		{
			lock (ServerRegions)
			{
				int index = ServerRegions.FindIndex(region => region.Name == name);
				if (index != -1)
				{
					ServerRegions.RemoveAt(index);
					foreach (var player in ServerSideCharacter.XmlData.Data)
					{
						int id = player.Value.ownedregion.FindIndex(region => region.Name == name);
						if (id != -1)
						{
							player.Value.ownedregion.RemoveAt((id));
						}
					}
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public bool HasNameConflect(string name)
		{
			lock (ServerRegions)
			{
				foreach (var region in ServerRegions)
				{
					if (name.Equals(region.Name))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool CheckPlayerRegionMax(ServerPlayer player)
		{
			lock (ServerRegions)
			{
				return ServerRegions.Count(info => info.Owner.Equals(player)) < 3;
			}
		}

		public bool CheckRegionConflict(Rectangle rect)
		{
			lock (ServerRegions)
			{
				return ServerRegions.All(region => !region.Area.Intersects(rect));
			}
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
				XmlReaderSettings settings = new XmlReaderSettings {IgnoreComments = true};
				//忽略文档里面的注释
				XmlDocument xmlDoc = new XmlDocument();
				XmlReader reader = XmlReader.Create("SSC/regions.xml", settings);
				xmlDoc.Load(reader);
				XmlNode xn = xmlDoc.SelectSingleNode("Regions");
				var list = xn.ChildNodes;
				foreach (var node in list)
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
					ServerSideCharacter.RegionManager.CreateNewRegion(area, name, player);
				}
			}
		}

		public void WriteRegionInfo()
		{
			lock (ServerRegions)
			{
				XmlDocument xmlDoc = new XmlDocument();
				XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
				xmlDoc.AppendChild(node);
				XmlNode root = xmlDoc.CreateElement("Regions");
				xmlDoc.AppendChild(root);
				foreach (var region in ServerRegions)
				{
					XmlElement xe = (XmlElement) xmlDoc.CreateNode(XmlNodeType.Element, "Region", null);
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

		}

		public bool CheckRegion(int X, int Y, ServerPlayer player)
		{
			lock (ServerRegions)
			{
				Vector2 tilePos = new Vector2(X, Y);
				foreach (var regions in ServerRegions)
				{
					if (regions.Area.Contains(X, Y) && !regions.Owner.Equals(player) && !regions.SharedOwner.Contains(player.Hash))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool CheckRegionSize(ServerPlayer player, Rectangle area)
		{
			return player.PermissionGroup.IsSuperAdmin() || (area.Width < 35 && area.Height < 35);
		}

		public void ShareRegion(ServerPlayer p, ServerPlayer target, string name)
		{
			int index = ServerRegions.FindIndex(region => region.Name == name);
			if (index == -1)
			{
				p.SendErrorInfo("Cannot find this region!");
				return;
			}
			var reg = ServerRegions[index];
			reg.SharedOwner.Add(target.Hash);
			p.SendSuccessInfo("Successfully shared " + reg.Name + " to " + target.Name);
			target.SendSuccessInfo(p.Name + " shared region " + reg.Name + " with you!");
		}

		internal bool ValidRegion(ServerPlayer player, string name, Rectangle area)
		{
			return !HasNameConflect(name) && ServerRegions.Count < 512
			       && CheckPlayerRegionMax(player) && CheckRegionConflict(area)
			       && CheckRegionSize(player, area);

		}
	}
}
