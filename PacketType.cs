namespace ServerSideCharacter
{
	public enum SSCMessageType
	{
		SyncPlayerHealth,
		SyncPlayerMana,
		SyncPlayerBank,
		RequestSaveData,
		RequestRegister,
		RequestSetGroup,
		RequestItem,
		RequestAuth,
		ResetPassword,
		SendLoginPassword,
		SendTimeSet,
		HelpCommand,
		KillCommand,
		ListCommand,
		SummonCommand,
		TPHereCommand,
		ButcherCommand,
		BanItemCommand,
		TPCommand,
		TimeCommand,
		ToggleExpert,
		ToggleHardMode,
		ToggleXmas,
		RegionCreateCommand,
		RegionRemoveCommand,
		RegionShareCommand,
		LockPlayer,
		TeleportPalyer,
		ToggleGodMode,
		SetGodMode,
		ServerSideCharacter,
		GenResources,
		ChestCommand,
		TPProtect
	}

	public enum GenerationType
	{
		Tree,
		Chest,
		Ore,
		Trap
	}
}
