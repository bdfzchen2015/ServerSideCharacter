using System;

namespace ServerSideCharacter.Extensions
{
	public static class StringExtension
	{
		public static bool Contains(this string source, string value, StringComparison comparrisonType) //Faster than String.ToLower().Contains(String.ToLower());
		{
			return source != null && value != null && source.IndexOf(value, comparrisonType) >= 0;
		}
	}
}
