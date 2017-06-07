using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ServerSideCharacter.Region
{
	public class RegionInfoData
	{
		public List<RegionInfo> ServerRegions = new List<RegionInfo>();
	}

	public class RegionManager
	{
		private RegionInfoData _regions = new RegionInfoData();
		private static string _filePath = "SSC/regions.json";

		public void CreateNewRegion(Rectangle rect, string name, ServerPlayer player)
		{
			lock (_regions)
			{
				RegionInfo playerRegion = new RegionInfo(name, player, rect);
				_regions.ServerRegions.Add(playerRegion);
				player.ownedregion.Add(playerRegion);
			}
		}

		public bool RemoveRegionWithName(string name)
		{
			lock (_regions)
			{
				int index = _regions.ServerRegions.FindIndex(region => region.Name == name);
				if (index != -1)
				{
					_regions.ServerRegions.RemoveAt(index);
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
			lock (_regions)
			{
				foreach (var region in _regions.ServerRegions)
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
			lock (_regions)
			{
				return _regions.ServerRegions.Count(info => info.Owner.Equals(player)) < ServerSideCharacter.Config.PlayerMaxRegions;
			}
		}

		public bool CheckRegionConflict(Rectangle rect)
		{
			lock (_regions)
			{
				return _regions.ServerRegions.All(region => !region.Area.Intersects(rect));
			}
		}

		public void ReadRegionInfo()
		{
			if (!File.Exists(_filePath))
			{
				string json = JsonConvert.SerializeObject(_regions);
				using (StreamWriter sw = new StreamWriter(_filePath))
				{
					sw.Write(json);
				}
			}
			else
			{
				string json = File.ReadAllText(_filePath);
				_regions = JsonConvert.DeserializeObject<RegionInfoData>(json);
				//XmlReaderSettings settings = new XmlReaderSettings {IgnoreComments = true};
				////忽略文档里面的注释
				//XmlDocument xmlDoc = new XmlDocument();
				//XmlReader reader = XmlReader.Create("SSC/regions.xml", settings);
				//xmlDoc.Load(reader);
				//XmlNode xn = xmlDoc.SelectSingleNode("Regions");
				//var list = xn.ChildNodes;
				//foreach (var node in list)
				//{
				//	XmlElement regionData = (XmlElement)node;

				//	int uuid = int.Parse(regionData.GetAttribute("uuid"));
				//	var info = regionData.ChildNodes;
				//	Rectangle area = new Rectangle();
				//	string name = info.Item(0).InnerText;
				//	area.X = Convert.ToInt32(info.Item(1).InnerText);
				//	area.Y = Convert.ToInt32(info.Item(2).InnerText);
				//	area.Width = Convert.ToInt32(info.Item(3).InnerText);
				//	area.Height = Convert.ToInt32(info.Item(4).InnerText);
				//	ServerPlayer player = ServerPlayer.FindPlayer(uuid);
				//	ServerSideCharacter.RegionManager.CreateNewRegion(area, name, player);
				//}
			}
		}

		public void WriteRegionInfo()
		{
			lock (_regions)
			{
				string json = JsonConvert.SerializeObject(_regions, Newtonsoft.Json.Formatting.Indented);
				using (StreamWriter sw = new StreamWriter(_filePath))
				{
					sw.Write(json);
				}
			}

		}

		public bool CheckRegion(int X, int Y, ServerPlayer player)
		{
			lock (_regions)
			{
				Vector2 tilePos = new Vector2(X, Y);
				foreach (var regions in _regions.ServerRegions)
				{
					if (regions.Area.Contains(X, Y) && !regions.Owner.Equals(player) && !regions.SharedOwner.Contains(player.UUID))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool CheckRegionSize(ServerPlayer player, Rectangle area)
		{
			return player.PermissionGroup.IsSuperAdmin() || (area.Width < ServerSideCharacter.Config.MaxRegionWidth
				&& area.Height < ServerSideCharacter.Config.MaxRegionHeigth);
		}

		public void ShareRegion(ServerPlayer p, ServerPlayer target, string name)
		{
			int index = _regions.ServerRegions.FindIndex(region => region.Name == name);
			if (index == -1)
			{
				p.SendErrorInfo("Cannot find this region!");
				return;
			}
			var reg = _regions.ServerRegions[index];
			reg.SharedOwner.Add(target.UUID);
			p.SendSuccessInfo("Successfully shared " + reg.Name + " to " + target.Name);
			target.SendSuccessInfo(p.Name + " shared region " + reg.Name + " with you!");
		}

		internal bool ValidRegion(ServerPlayer player, string name, Rectangle area)
		{
			return !HasNameConflect(name) && _regions.ServerRegions.Count < ServerSideCharacter.Config.MaxRegions
				   && CheckPlayerRegionMax(player) && CheckRegionConflict(area)
				   && CheckRegionSize(player, area);

		}

		public List<RegionInfo> GetList()
		{
			return _regions.ServerRegions;
		}


	}
}
