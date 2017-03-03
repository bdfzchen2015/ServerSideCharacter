using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ServerSideCharacter
{
	public class TextLog : IDisposable
	{
		private readonly StreamWriter _logWriter;

		public string FileName { get; set; }

		public TextLog(string filename, bool clear)
		{
			FileName = filename;
			_logWriter = new StreamWriter(filename, !clear);
		}

		public void WriteToFile(string msg)
		{
			_logWriter.WriteLine(msg);
		}
		
		public void Dispose()
		{
			_logWriter.Dispose();
		}
	}
}
