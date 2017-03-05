using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSideCharacter
{
	public class Command
	{
		public Action<string[]> CommandAction;
		public string Name { get; set; }
		public string Description { get; set; }

		public Command(string name, Action<string[]> action, string description)
		{
			Name = name;
			CommandAction = action;
			Description = description;
		}
	}
}
