using System;
using System.Collections.Generic;
using System.Xml;

namespace ServerSideCharacter
{
	public class PlayerDatas
	{
		public Dictionary<string, ServerPlayer> Data = new Dictionary<string, ServerPlayer>();

		public PlayerDatas()
		{

		}

		private string ReadNext(XmlNodeList info, ref int i)
		{
			return info.Item(i++).InnerText;
		}

		public PlayerDatas(string path)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreComments = true;                         //忽略文档里面的注释
			XmlDocument xmlDoc = new XmlDocument();
			XmlReader reader = XmlReader.Create(path, settings);
			xmlDoc.Load(reader);
			XmlNode xn = xmlDoc.SelectSingleNode("Players");
			var list = xn.ChildNodes;
			foreach (var node in list)
			{
				XmlElement pData = (XmlElement)node;
				ServerPlayer player = new ServerPlayer();
				var info = pData.ChildNodes;
				int i = 0;
				player.Name = ReadNext(info, ref i);
				player.Password = ReadNext(info, ref i);
				player.Hash = ReadNext(info, ref i);
				player.LifeMax = Convert.ToInt32(ReadNext(info, ref i));
				player.StatLife = Convert.ToInt32(ReadNext(info, ref i));
				player.ManaMax = Convert.ToInt32(ReadNext(info, ref i));
				player.StatMana = Convert.ToInt32(ReadNext(info, ref i));
				for (int id = 0; id < 59; id++)
				{
					int type = Convert.ToInt32(ReadNext(info, ref i));
					if (type != 0)
					{
						player.inventroy[id].SetDefaults(type);
						player.inventroy[id].prefix =
							Convert.ToByte((list.Item(i) as XmlElement).GetAttribute("prefix"));
						player.inventroy[id].stack =
							Convert.ToInt32((list.Item(i) as XmlElement).GetAttribute("stack"));
					}
				}
				for (int id = 59; id < 79; id++)
				{
					int type = Convert.ToInt32(ReadNext(info, ref i));
					if (type != 0)
					{
						player.armor[id - 59].SetDefaults(type);
						player.inventroy[id].prefix =
							Convert.ToByte((list.Item(i) as XmlElement).GetAttribute("prefix"));
					}
				}
				for (int id = 79; id < 89; id++)
				{
					int type = Convert.ToInt32(ReadNext(info, ref i));
					if (type != 0)
					{
						player.dye[id - 79].SetDefaults(type);
					}
				}
				for (int id = 89; id < 94; id++)
				{
					int type = Convert.ToInt32(ReadNext(info, ref i));
					if (type != 0)
					{
						player.miscEquips[id - 89].SetDefaults(type);
					}
				}
				for (int id = 94; id < 99; id++)
				{
					int type = Convert.ToInt32(ReadNext(info, ref i));
					if (type != 0)
					{
						player.miscDye[id - 94].SetDefaults(type);
					}
				}
				Data.Add(player.Name, player);
			}
			reader.Close();
		}
	}
}
