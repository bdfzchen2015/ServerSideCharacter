using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSideCharacter
{
	public class Command
	{
		public Action<string[]> CommandAction;
		public string Name;
		
		public Command(string name, Action<string[]> action)
		{
			Name = name;
			CommandAction = action;
		}
	}
}
