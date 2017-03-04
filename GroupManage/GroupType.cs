using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSideCharacter.GroupManage
{
	public static class GroupType
	{

		public static Dictionary<string, Group> Groups = new Dictionary<string, Group>();
		
		internal static void SetupGroups()
		{
			Group DefaultGroup = new Group("default");
			DefaultGroup.permissions.Add(new PermissionInfo("tp", "Teleport player"));
			DefaultGroup.permissions.Add(new PermissionInfo("ls", "List all player's info"));
			Groups.Add(DefaultGroup.GroupName, DefaultGroup);


			Group Admin = new Group("admin");
			Admin.permissions = new List<PermissionInfo>(DefaultGroup.permissions);
			Admin.permissions.Add(new PermissionInfo("time", "Changing times"));
			Admin.permissions.Add(new PermissionInfo("butcher", "Kill all monsters"));
			Groups.Add(Admin.GroupName, Admin);


			Group SuperAdmin = new Group("spadmin");
			SuperAdmin.permissions = new List<PermissionInfo>(Admin.permissions);
			Groups.Add(SuperAdmin.GroupName, SuperAdmin);
		}

	}
}
