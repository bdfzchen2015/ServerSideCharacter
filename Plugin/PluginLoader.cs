using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using ServerSideCharacter.ServerCommand;

namespace ServerSideCharacter.Plugin
{
	public static class PluginLoader
	{
		public static void LoadPlugins()
		{
			try
			{
				//获取程序的基目录
				string path = AppDomain.CurrentDomain.BaseDirectory;
				path = Path.Combine(path, "Plugins");
				string[] files = Directory.GetFiles(path, "*.dll");
				if (files.Length < 1) return;
				foreach (var plugin in files)
				{
					Assembly asm = Assembly.LoadFile(plugin);
					foreach (var type in asm.GetTypes())
					{
						if (type.GetInterface("IPluginBase") != null)
						{
							LoadPluginBase(type);
						}
					}
				}
				Console.WriteLine("Loaded all plugins!");
			}
			catch (Exception ex)
			{
				Console.Write(ex);
			}
		}

		private static void LoadPluginBase(Type type)
		{

			IPluginBase plugin = (IPluginBase)Activator.CreateInstance(type);
			if (plugin == null)
			{
				throw new Exception("Cannot create the instance of plugin: " + type.Name);
			}
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Plugin Name: " + plugin.ModName + " Author: " + plugin.AuthorName);
			Console.ResetColor();
		}
	}
}
