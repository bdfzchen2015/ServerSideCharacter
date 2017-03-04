using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSideCharacter.Region
{
	public class RegionInfo
	{
		public string Name { get; set; }
		public ServerPlayer Owner { get; set; }
		public List<ServerPlayer> SharedOwner { get; set; }
		public Rectangle Area { get; set; }
		public List<RegionPermission> permissions { get; set; }

		public RegionInfo(string name, ServerPlayer player, Point pos1, Point pos2)
		{
			Name = name;
			Owner = player;
			Area = new Rectangle();
		}
	}
}
