using System;
using System.IO;

namespace ServerSideCharacter
{
	public class ErrorLogger : IDisposable
	{
		private readonly StreamWriter _logWriter;

		public string FileName { get; set; }

		public ErrorLogger(string filename, bool clear)
		{
			FileName = filename;
			_logWriter = new StreamWriter(filename, !clear);
		}

		public void WriteToFile(string msg)
		{
			_logWriter.WriteLine(msg);
			_logWriter.Flush();
		}

		public void Dispose()
		{
			_logWriter.Dispose();
		}
	}
}
