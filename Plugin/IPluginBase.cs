using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServerSideCharacter.Plugin
{
	public interface IPluginBase
	{
		string ModName { get; }
		string AuthorName { get; }
	}
}
