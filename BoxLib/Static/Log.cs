using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BoxLib.Static
{
	/// <summary>
	/// This class is responsible to write advanced logs to both the console and a file stream.
	/// </summary>
	public static class Log
	{
		/// <summary>
		/// The folder where all log files are stored.
		/// </summary>
		private const string LogFolder = "Logs";

		/// <summary>
		/// The current log file that is written during application lifetime.
		/// </summary>
		private static readonly string LogFile = $"{DateTime.Today:yy-MM-dd}.log";

		/// <summary>
		/// The maximum amount of bytes that are allowed in the logs folder before old files will get deleted.
		/// </summary>
		public static long MaxBytes = 10485760;

		/// <summary>
		/// Enables or disables log output.
		/// </summary>
		public static bool Enabled = true;

		/// <summary>
		/// Enables or disables verbose messages that may only be relevant for debugging.
		/// </summary>
		public static bool ShowVerbose = true;

		/// <summary>
		/// The standard file logger to use.
		/// </summary>
		private static readonly TextWriterTraceListener _fileLogger = new TextWriterTraceListener(Path.Combine(LogFolder, LogFile));

		/// <summary>
		/// Logs a message with the specified indent and verbosity. Returns whether or not the logging was successful.
		/// </summary>
		/// <param name="message">The message to log.</param>
		/// <param name="indent">The indent used inside the log for this message.</param>
		/// <param name="v">The kind of verbosity for this message.</param>
		/// <param name="noFlush">Set to true to queue this message for the next flush.</param>
		public static bool Write(string message, int indent, TraceEventType? v, bool noFlush = false)
		{
			if(!Enabled)
				return true;

			try
			{
				if(!Directory.Exists(LogFolder))
					Directory.CreateDirectory(LogFolder);
			}
			catch(Exception)
			{
				return false;
			}

			if(!Trace.Listeners.Contains(_fileLogger))
			{
				bool noFile = !File.Exists(Path.Combine(LogFolder, LogFile));

				Trace.Listeners.Add(_fileLogger);

				if(!noFile)
					Trace.WriteLine("");

				Trace.WriteLine($"Start of application lifetime - {Process.GetCurrentProcess().StartTime:T}");
			}

			if(!ShowVerbose && v == TraceEventType.Verbose)
				return true;

			Trace.IndentLevel = indent;

			string[] allMessages = message.Split(new[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);

			for(int i = 0; i < allMessages.Length; i++)
			{
				Trace.WriteLine(i == allMessages.Length - 1 ? $"{allMessages[i]} - {DateTime.Now:T}" : $"{allMessages[i]}",
					v?.ToString().ToUpper());
			}

			if(!noFlush)
				Trace.Flush();

			return true;
		}

		/// <summary>
		/// Closes this logging session. Only use this at the very end of application runtime.
		/// </summary>
		public static void Close()
		{
			Trace.IndentLevel = 0;
			Trace.WriteLine($"End of application lifetime - {DateTime.Now:T}");

			Trace.Flush();
			Trace.Close();

			FileInfo[] logfiles = Directory.GetFiles(LogFolder, "*.*", SearchOption.AllDirectories).Select(a => new FileInfo(a)).ToArray();
			long totalBytes = 0;
			for(int i = 0; i < logfiles.Length; i++)
			{
				totalBytes += logfiles[i].Length;
			}

			while(totalBytes > MaxBytes)
			{
				FileInfo oldFile = logfiles.OrderBy(a => a.LastWriteTimeUtc).FirstOrDefault();

				totalBytes -= oldFile?.Length ?? 0;

				oldFile?.Delete();
			}
		}
	}
}