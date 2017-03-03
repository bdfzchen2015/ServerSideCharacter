using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ServerSideCharacter.ServerCommand
{
	public static class CommandBoardcast
	{ 

		public static void ShowSaveInfo()
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string info = string.Format("[SSC {0}] Saved all player data", ServerSideCharacter.Version);
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}

		public static void ShowSavePlayer(ServerPlayer p)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string info = string.Format("[SSC {0}] Saved {1}'s data", ServerSideCharacter.Version, p.Name);
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}
		public static void ShowMessage(string msg)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			string info = string.Format("[SSC {0}] {1}", ServerSideCharacter.Version, msg);
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}
		public static void ShowError(Exception ex)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			string info = string.Format("[SSC {0}] {1}", ServerSideCharacter.Version, ex.ToString());
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}
		public static void ShowError(string msg)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			string info = string.Format("[SSC {0}] {1}", ServerSideCharacter.Version, msg);
			Console.WriteLine(info);
			LogInfo(info);
			Console.ResetColor();
		}
		public static void LogInfo(string msg)
		{
			string dateTime = DateTime.Now.ToLongTimeString();
			string text = dateTime + "\n" + msg + "\n";
			ServerSideCharacter.Logger.WriteToFile(text);
		}
	}
}
