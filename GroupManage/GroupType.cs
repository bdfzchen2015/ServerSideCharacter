using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSideCharacter.GroupManage
{
	public static class GroupType
	{

		public static Dictionary<string, Group> Groups = new Dictionary<string, Group>();
		
		private static void AddToGroup(Group g)
		{
			Groups.Add(g.GroupName, g);
		}

		internal static void SetupGroups()
		{
			Group CrminalGroup = new Group("criminal");
			CrminalGroup.ChatColor = Color.Gray;
			CrminalGroup.ChatPrefix = "Criminal";
			AddToGroup(CrminalGroup);

			Group DefaultGroup = new Group("default");
			DefaultGroup.permissions.Add(new PermissionInfo("tp", "Teleport player"));
			DefaultGroup.permissions.Add(new PermissionInfo("ls", "List online player's info"));
			DefaultGroup.permissions.Add(new PermissionInfo("auth", "Authorize as super admin"));
			AddToGroup(DefaultGroup);


			Group Admin = new Group("admin");
			Admin.ChatColor = Color.Red;
			Admin.ChatPrefix = "Admin";
			Admin.permissions = new List<PermissionInfo>(DefaultGroup.permissions);
			Admin.permissions.Add(new PermissionInfo("time", "Changing times"));
			Admin.permissions.Add(new PermissionInfo("butcher", "Kill all monsters"));
			Admin.permissions.Add(new PermissionInfo("ls -al", "List all player's info"));
			Admin.permissions.Add(new PermissionInfo("lock", "Lock a player"));
			Admin.permissions.Add(new PermissionInfo("sm", "Summon monsters"));
			Admin.permissions.Add(new PermissionInfo("tphere", "Force teleport a player to your place"));
			Admin.permissions.Add(new PermissionInfo("region", "Manage regions"));
			AddToGroup(Admin);


			Group SuperAdmin = new Group("spadmin");
			SuperAdmin.ChatColor = Color.Cyan;
			SuperAdmin.ChatPrefix = "Super Admin";
			SuperAdmin.permissions = new List<PermissionInfo>(Admin.permissions);
			SuperAdmin.permissions.Add(new PermissionInfo("group", "Manage group"));
			AddToGroup(SuperAdmin);
		}

	}
}
