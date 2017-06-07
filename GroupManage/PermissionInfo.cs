namespace ServerSideCharacter.GroupManage
{
	public class PermissionInfo
	{
		public string Description { get; set; }
		public string Name { get; set; }

		public PermissionInfo(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}
}
