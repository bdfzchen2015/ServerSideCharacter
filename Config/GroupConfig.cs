using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerSideCharacter.GroupManage;
using ServerSideCharacter.ServerCommand;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServerSideCharacter.Config.Group
{

	public class ConfigData
	{
		public GroupType GroupType;
		public ConfigData()
		{
			GroupType = new GroupType();
		}
	}
	public class GroupConfigManager
	{
		private ConfigConverter converter = new ConfigConverter();
		public bool ConfigExist
		{
			get
			{
				return _jsonExist;
			}
		}

		public Dictionary<string, GroupManage.Group> Groups
		{
			get
			{
				return _configData.GroupType.Groups;
			}
		}
		private static string _configPath = "SSC/groups.json";

		private ConfigData _configData;

		private bool _jsonExist;



		public GroupConfigManager()
		{
			_jsonExist = File.Exists(_configPath);
			SetConfig();
		}

		private void SetConfig()
		{
			if (!ConfigExist)
			{
				_configData = new ConfigData();
				_configData.GroupType.SetupGroups();
				string data = JsonConvert.SerializeObject(_configData, Formatting.Indented, converter);
				using (StreamWriter sw = new StreamWriter(_configPath))
				{
					sw.Write(data);
				}
				CommandBoardcast.ConsoleMessage("Group config file created.");
			}
			else
			{
				using (StreamReader sr = new StreamReader(_configPath))
				{
					string data = sr.ReadToEnd();
					_configData = JsonConvert.DeserializeObject<ConfigData>(data, converter);

					_configData.GroupType.SetupGroups(!_configData.GroupType.Groups.ContainsKey("default"), !_configData.GroupType.Groups.ContainsKey("criminal"), false, !_configData.GroupType.Groups.ContainsKey("spadmin")); //Add default, criminal and spadmin group if not exists
					_configData.GroupType.Groups["spadmin"].permissions.Clear(); //clear all spadmin user created permissions
					_configData.GroupType.Groups["spadmin"].permissions.Add(new PermissionInfo("all", "all commands")); //add all permissions to spadmin group
				}
			}
		}

		private void AddGroup(GroupManage.Group group)
		{
			_configData.GroupType.Groups.Add(group.GroupName, group);
		}

		public void Save()
		{
			string data = JsonConvert.SerializeObject(_configData, Formatting.Indented, converter);
			using (StreamWriter sw = new StreamWriter(_configPath))
			{
				sw.Write(data);
			}
		}

	}
	public class ConfigConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(ConfigData));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{

			Dictionary<string, GroupManage.Group> data = serializer.Deserialize<Dictionary<string, GroupManage.Group>>(reader);
			ConfigData config = new ConfigData();
			for (int i = 0; i < data.Count; i++)
			{
				var pair = data.ElementAt(i);
				pair.Value.GroupName = pair.Key;
				data[pair.Key] = pair.Value;
			}
			config.GroupType.Groups = data;
			return config;
		}
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			ConfigData config = (ConfigData)value;
			JObject obj = JObject.FromObject(config.GroupType.Groups);
			obj.WriteTo(writer);
		}
	}
}
