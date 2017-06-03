using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSideCharacter.GroupManage
{
	public class Group
	{
		[JsonIgnore]
		public string GroupName { get; set; }
		public List<PermissionInfo> permissions = new List<PermissionInfo>();
		public Color ChatColor = new Color();
		public string ChatPrefix = "";

		public Group(string name)
		{
			GroupName = name;
			ChatColor = Color.White;
		}

		public bool IsSuperAdmin()
		{
			return GroupName == "spadmin";
		}

		public bool HasPermission(string name)
		{
			return GroupName == "spadmin" || permissions.Any(t => t.Name == name);
		}
	}
}
