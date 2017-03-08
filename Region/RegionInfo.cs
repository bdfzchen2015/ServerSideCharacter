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
		public List<ServerPlayer> SharedOwner = new List<ServerPlayer>();
		public Rectangle Area { get; set; }
		public List<RegionPermission> Permissions { get; set; }

		public RegionInfo(string name, ServerPlayer player, Rectangle rect)
		{
			Name = name;
			Owner = player;
			Area = rect;
		}

		public string WelcomeInfo()
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(string.Format("Welcome to region '{0}'!", Name));
			sb.AppendLine(string.Format("*Region Owner: {0}", Owner.Name));
			sb.AppendLine(string.Format("*Region Area: {0}", Area.ToString()));
			return sb.ToString();
		}

		public string LeaveInfo()
		{
			return string.Format("You have left '{0}'", Name);
		}
	}
}
