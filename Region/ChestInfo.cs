using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace ServerSideCharacter.Region
{
	public class ChestManager
	{
		[Flags]
		public enum Pending
		{
			Protect = 1,
			DeProtect = 2,
			Public = 4,
			UnPublic = 8,
			AddFriend = 16,
			RemoveFriend = 32
		}
		public List<ChestInfo> ChestInfo = new List<ChestInfo>();
		public Dictionary<int, Pending> Pendings = new Dictionary<int, Pending>();
		private Dictionary<int, int> friendPendings = new Dictionary<int, int>();
		public ChestManager Initialize()
		{
			for (int i = 0; i < Main.chest.Length; i++)
			{
				ChestInfo.Add(new ChestInfo());
			}
			return this;
		}
		private void SetFriendP(ServerPlayer player, ServerPlayer friend)
		{
			if (friendPendings.ContainsKey(player.UUID))
				if (friend == null)
					friendPendings.Remove(player.UUID);
				else
					friendPendings[player.UUID] = friend.UUID;
			else if (friend != null)
				friendPendings.Add(player.UUID, friend.UUID);
		}
		public ServerPlayer GetFriendP(ServerPlayer player)
		{
			int uuid = friendPendings.ContainsKey(player.UUID) == true ? friendPendings[player.UUID] : -1;
			try
			{
				player.SendInfo(uuid.ToString());
			}
			catch (Exception ex)
			{
				player.SendErrorInfo(ex.ToString());
			}
			Console.WriteLine(uuid);
			return ServerPlayer.FindPlayer(uuid);

		}
		public void AddPending(ServerPlayer player, Pending pending, ServerPlayer friend = null)
		{

			if (Pendings.ContainsKey(player.UUID))
				Pendings[player.UUID] |= pending;
			else
				Pendings.Add(player.UUID, pending);
			if (pending.HasFlag(Pending.AddFriend) || pending.HasFlag(Pending.RemoveFriend))
				SetFriendP(player, friend);
		}
		public void SetPendings(ServerPlayer player, Pending pending, ServerPlayer friend = null)
		{
			if (Pendings.ContainsKey(player.UUID))
				Pendings[player.UUID] = pending;
			else
				Pendings.Add(player.UUID, pending);
			if (pending.HasFlag(Pending.AddFriend) || pending.HasFlag(Pending.RemoveFriend))
				SetFriendP(player, friend);

		}
		public void RemovePending(ServerPlayer player, Pending pending)
		{
			if (Pendings.ContainsKey(player.UUID))
				Pendings[player.UUID] &= ~pending;
			if (pending.HasFlag(Pending.AddFriend) || pending.HasFlag(Pending.RemoveFriend))
				SetFriendP(player, null);
		}
		public void AddFriend(int chestID, ServerPlayer friend)
		{
			ChestInfo[chestID].AddFriend(friend);
		}
		public void RemoveFriend(int chestID, ServerPlayer friend)
		{
			ChestInfo[chestID].RemoveFriend(friend);
		}
		public void RemoveAllPendings(ServerPlayer player)
		{
			if (Pendings.ContainsKey(player.UUID))
				Pendings[player.UUID] = new Pending();
			SetFriendP(player, null);
		}
		public Pending GetPendings(ServerPlayer player)
		{
			return Pendings.ContainsKey(player.UUID) == true ? Pendings[player.UUID] : new Pending();

		}
		public void SetOwner(int chestID, int ownerID, bool isPublic)
		{
			ChestInfo[chestID].OwnerID = ownerID;
			ChestInfo[chestID].IsPublic = isPublic;
		}

		public bool IsNull(int chestID)
		{
			var id = ChestInfo[chestID].OwnerID;
			return id == -1;
		}
		public bool IsOwner(int chestID, ServerPlayer player)
		{
			var id = ChestInfo[chestID].OwnerID;
			return id == player.UUID;
		}
		public bool IsPublic(int chestID)
		{
			var isPublic = ChestInfo[chestID].IsPublic;
			return isPublic;
		}
		public bool CanOpen(int chestID, ServerPlayer player)
		{
			var id = ChestInfo[chestID].OwnerID;
			var isPublic = ChestInfo[chestID].IsPublic;
			var friends = ChestInfo[chestID].Friends;
			return id == -1 || id == player.UUID || player.PermissionGroup.HasPermission("chest") || isPublic || friends.Contains(player.UUID);
		}
	}
	public class ChestInfo
	{
		private int ownerID = -1;
		private bool isPublic = false;
		private List<int> friends = new List<int>();
		public ChestInfo()
		{

		}
		public void AddFriend(ServerPlayer player)
		{
			if (!friends.Contains(player.UUID))
				friends.Add(player.UUID);
		}
		public void RemoveFriend(ServerPlayer player)
		{
			if (friends.Contains(player.UUID))
				friends.RemoveAll(id => id == player.UUID);
		}
		public int OwnerID
		{
			get { return ownerID; }
			set
			{
				if (value == -1)
				{
					isPublic = false;
					friends.Clear();
				}
				ownerID = value;
			}
		}
		public bool IsPublic
		{
			get { return isPublic; }
			set { isPublic = value; }
		}
		public List<int> Friends
		{
			get { return friends; }
		}
	}
}
