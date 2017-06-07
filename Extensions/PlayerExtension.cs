using System;
using Terraria;


namespace ServerSideCharacter
{
	public static class PlayerExtension
	{
		public static ServerPlayer GetServerPlayer(this Player p)
		{
			if (ServerSideCharacter.XmlData.Data.ContainsKey(p.name))
			{
				return ServerSideCharacter.XmlData.Data[p.name];
			}
			else
			{
				throw new ArgumentException("Player name not found!");
			}
		}
	}
}
