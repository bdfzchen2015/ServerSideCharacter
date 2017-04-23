using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ServerSideCharacter.ServerCommand;

namespace ServerSideCharacter.Config
{
	/// <summary>
	/// Server config file using json
	/// </summary>
	public class ConfigData
	{
		public List<NetItem> StartUpItems;
		public List<NetItem> BannedItems;

		public ConfigData()
		{
			StartUpItems = new List<NetItem>();
			BannedItems = new List<NetItem>();
		}
	}

	public class ServerConfigManager
	{

		public bool ConfigExist
		{
			get
			{
				return _jsonExist;
			}
		}

		public List<NetItem> StartupItems
		{
			get
			{
				return _configData.StartUpItems;
			}
		}

		private static string _configPath = "SSC/config.json";

		private ConfigData _configData;

		private bool _jsonExist;



		public ServerConfigManager()
		{
			_jsonExist = File.Exists(_configPath);
			SetConfig();
		}

		private void SetConfig()
		{
			if (!ConfigExist)
			{
				_configData = new ConfigData();
				AddToStartInv(ItemID.ShadewoodSword, 82);
				AddToStartInv(ItemID.IronPickaxe, 83);
				AddToStartInv(ItemID.IronAxe, 81);
				AddToBannedItem(ItemID.IronAxe);
				string data = JsonConvert.SerializeObject(_configData, Formatting.Indented);
				using(StreamWriter sw = new StreamWriter(_configPath))
				{
					sw.Write(data);
				}
				CommandBoardcast.ConsoleMessage("Config file created.");
			}
			else
			{
				using(StreamReader sr = new StreamReader(_configPath))
				{
					string data = sr.ReadToEnd();
					_configData = JsonConvert.DeserializeObject<ConfigData>(data);
				}
			}
			SetUpStartInv();
		}

		private void SetUpStartInv()
		{
			if (ModLoader.LoadedMods.Any(mod => mod.Name == "ThoriumMod"))
			{
				var thorium = ModLoader.LoadedMods.Where(mod => mod.Name == "ThoriumMod");
				AddToStartInv(thorium.First().ItemType("FamilyHeirloom"));
			}
		}



		private void AddToStartInv(int type, int prefex = 0)
		{
			Item item = new Item();
			item.SetDefaults(type);
			item.Prefix(prefex);
			_configData.StartUpItems.Add(Utils.ToNetItem(item));
		}

		private void AddToBannedItem(int type)
		{
			_configData.BannedItems.Add(Utils.ToNetItem(type));
		}

		public void Save()
		{
			string data = JsonConvert.SerializeObject(_configData, Formatting.Indented);
			using (StreamWriter sw = new StreamWriter(_configPath))
			{
				sw.Write(sw);
			}
		}

		public bool IsItemBanned(Item item, ServerPlayer player)
		{
			if(player.PermissionGroup.GroupName == "spadmin")
			{
				return false;
			}
			bool banned = false;
			if(_configData.BannedItems.Any(nitem => nitem.TheSameItem(item)))
			{
				banned = true;
			}
			return banned;
		}

		public void ToggleItemBan(int type, ServerPlayer player)
		{
			Item item = new Item();
			item.netDefaults(type);
			if (_configData.BannedItems.Any(nitem => nitem.TheSameItem(item)))
			{
				_configData.BannedItems.RemoveAll(nitem => nitem.TheSameItem(item));
				player.SendSuccessInfo("Now the item " + item.name + " is unbanned!");
			}
			else
			{
				_configData.BannedItems.Add(Utils.ToNetItem(item));
				player.SendSuccessInfo("You have successfully banned " + item.name + ".");
			}
		}
	}
}
