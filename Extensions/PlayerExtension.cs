using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;


namespace ServerSideCharacter
{
	public static class PlayerExtension
	{
		public static ServerPlayer GetServerPlayer(this Player p)
		{
			return ServerSideCharacter.XmlData.Data[p.name];
		}
	}
}
