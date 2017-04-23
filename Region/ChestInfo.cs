using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace ServerSideCharacter.Region
{
	public class ChestManager
	{
		public List<ChestInfo> ChestInfo = new List<ChestInfo>();

		public ChestManager Initialize()
		{
			for (int i = 0; i < Main.chest.Length; i++)
			{
				ChestInfo.Add(new ChestInfo());
			}
			return this;
		}

		public void SetOwner(int chestID, int ownerID)
		{
			ChestInfo[chestID].OwnerID = ownerID;
		}

		public bool IsNull(int chestID)
		{
			var id = ChestInfo[chestID].OwnerID;
			return id == -1;
		}

		public bool CanOpen(int chestID, ServerPlayer player)
		{
			var id = ChestInfo[chestID].OwnerID;
			return id == -1 || id == player.UUID || player.PermissionGroup.HasPermission("chest");
		}
	}
	public class ChestInfo
	{
		private int ownerID = -1;

		public ChestInfo()
		{

		}

		public int OwnerID
		{
			get { return ownerID; }
			set { ownerID = value; }
		}


	}
}
