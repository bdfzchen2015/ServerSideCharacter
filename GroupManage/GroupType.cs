using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ServerSideCharacter.GroupManage
{
	public class GroupType
	{
		[JsonIgnore]
		public readonly Dictionary<string, Group> DefaultGroups = new Dictionary<string, Group>();
		public Dictionary<string, Group> Groups = new Dictionary<string, Group>();

		private void AddToGroup(Group g)
		{
			Groups.Add(g.GroupName, g);
		}
		public GroupType()
		{
			Group crminalGroup = new Group("criminal")
			{
				ChatColor = Color.Gray,
				ChatPrefix = "Criminal"
			};
			Group defaultGroup = new Group("default")
			{
				ChatPrefix = "Default"
			};
			defaultGroup.permissions.Add(new PermissionInfo("tp", "Teleport player"));
			defaultGroup.permissions.Add(new PermissionInfo("ls", "List online player's info"));
			defaultGroup.permissions.Add(new PermissionInfo("auth", "Authorize as super admin"));
			Group admin = new Group("admin")
			{
				ChatColor = Color.Red,
				ChatPrefix = "Admin",
				permissions = new List<PermissionInfo>(defaultGroup.permissions)
				{
					new PermissionInfo("time", "Changing times"),
					new PermissionInfo("butcher", "Kill all monsters"),
					new PermissionInfo("ls -al", "List all player's info"),
					new PermissionInfo("lock", "Lock a player"),
					new PermissionInfo("sm", "Summon monsters"),
					new PermissionInfo("tphere", "Force teleport a player to your place"),
					new PermissionInfo("region", "Manage regions"),
					new PermissionInfo("region-create", "Create region"),
					new PermissionInfo("region-remove", "Remove regions"),
					new PermissionInfo("expert", "toggle expert"),
					new PermissionInfo("hardmode", "toggle hardmode"),
					new PermissionInfo("region-share", "Share regions"),
					new PermissionInfo("ban-item", "Ban certain item"),
					new PermissionInfo("chest", "Open locked chest"),
					new PermissionInfo("gen-res", "Generate world resources")
				}
			};
			Group superAdmin = new Group("spadmin")
			{
				ChatColor = Color.Cyan,
				ChatPrefix = "Super Admin",
			};
			superAdmin.permissions.Add(new PermissionInfo("all", "all commands"));
			DefaultGroups.Add("default", defaultGroup);
			DefaultGroups.Add("criminal", crminalGroup);
			DefaultGroups.Add("admin", admin);
			DefaultGroups.Add("spadmin", superAdmin);
		}
		internal void SetupGroups(bool setupDefault = true, bool setupCriminal = true, bool setupAdmin = true, bool setupSpAdmin = true)
		{
			if (setupDefault)
				AddToGroup(DefaultGroups["default"]);
			if (setupCriminal)
				AddToGroup(DefaultGroups["criminal"]);
			if (setupAdmin)
				AddToGroup(DefaultGroups["admin"]);
			if (setupSpAdmin)
				AddToGroup(DefaultGroups["spadmin"]);

		}
	}
}
