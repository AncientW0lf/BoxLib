using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace BoxLib.Objects
{
	/// <summary>
	/// This class is a wrapper for the <see cref="Trace"/> class and provides easy-to-use
	/// methods for writing diagnostic information to one or more outputs. Supports colored console text.
	/// </summary>
	public class Log : IDisposable
	{
		/// <summary>
		/// Set this flag to enable or disable the <see cref="Write"/> method.
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// Get or set the format used to display <see cref="DateTime.Now"/>.
		/// </summary>
		public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

		/// <summary>
		/// Internal flag used to decide whether to automatically flush the written content to the underlying streams.
		/// </summary>
		public bool AutoFlush { get; set; } = true;

		/// <summary>
		/// Prevents or allows the <see cref="Write"/> method from writing content
		/// that is marked <see cref="TraceEventType.Verbose"/>.
		/// </summary>
		public bool ShowVerbose { get; set; }

		/// <summary>
		/// If this flag is set to true, the <see cref="DeleteOldLogs"/> will automatically
		/// be called with default parameters on <see cref="Dispose"/>.
		/// </summary>
		public bool DeleteOnDispose { get; set; } = true;

		/// <summary>
		/// Gets whether this object writes to the console or not.
		/// </summary>
		public bool UsesConsole { get; private set; }

		/// <summary>
		/// Gets whether this object writes to a file or not.
		/// </summary>
		public bool UsesLogFile { get; private set; }

		/// <summary>
		/// The folder the log file will be saved in.
		/// </summary>
		private string _logFolder;

		/// <summary>
		/// Contains a list of all used <see cref="TraceListener"/> for this object.
		/// </summary>
		private readonly List<TraceListener> _listeners = new List<TraceListener>();

		private readonly object _lock = new object();

		/// <summary>
		/// Initializes a new <see cref="Log"/> object.
		/// </summary>
		/// <param name="writeToConsole">Set to true to write content to <see cref="Console.Out"/>.</param>
		public Log(bool writeToConsole)
		{
			Initialize(writeToConsole);
		}

		/// <summary>
		/// Initializes a new <see cref="Log"/> object that will additionally write to a log file.
		/// </summary>
		/// <param name="writeToConsole">Set to true to write content to <see cref="Console.Out"/>.</param>
		/// <param name="logfolder">The folder in which to write a default log file.</param>
		public Log(bool writeToConsole, string logfolder)
		{
			Initialize(writeToConsole, logfolder);
		}

		/// <summary>
		/// Initializes a new <see cref="Log"/> object that will additionally write to a log file.
		/// </summary>
		/// <param name="writeToConsole">Set to true to write content to <see cref="Console.Out"/>.</param>
		/// <param name="logfolder">The folder in which to write the specified log file.</param>
		/// <param name="filename">The full name of the file to write to.</param>
		public Log(bool writeToConsole, string logfolder, string filename)
		{
			Initialize(writeToConsole, logfolder, filename);
		}

		/// <summary>
		/// Writes a message into this object's listeners.
		/// </summary>
		/// <param name="message">The message to write.</param>
		/// <param name="eventType">The type of event that this message is associated with.</param>
		public void Write(string message, TraceEventType? eventType = null)
		{
			lock(_lock)
			{
				//Returns if the message is verbose and verbose messages are disabled
				//Also returns if this object is disabled in general
				if(eventType == TraceEventType.Verbose && !ShowVerbose 
				   || !Enabled)
					return;

				//Temporarily turns off auto flush
				bool tempFlush = Trace.AutoFlush;
				Trace.AutoFlush = false;

				//Gets the current foreground color of the console window, if necessary
				ConsoleColor? prevColor = UsesConsole 
					? Console.ForegroundColor
					: (ConsoleColor?)null;

				Trace.Write('[');

				//Changes the console foreground color to highlight the current date/time
				if(UsesConsole)
					Console.ForegroundColor = ConsoleColor.Yellow;

				//Writes the current date/time in a specified format
				Trace.Write(DateTime.Now.ToString(DateTimeFormat));

				//Changes the console foreground color back to the previous value
				if(prevColor.HasValue)
					Console.ForegroundColor = prevColor.Value;

				Trace.Write(']');

				if(eventType != null)
				{
					Trace.Write(" [");

					//Changes the console foreground color depending on the current event type
					if(UsesConsole)
					{
						Console.ForegroundColor = eventType switch
						{
							TraceEventType.Information => ConsoleColor.Blue,
							TraceEventType.Critical => ConsoleColor.Magenta,
							TraceEventType.Error => ConsoleColor.Red,
							TraceEventType.Resume => ConsoleColor.Cyan,
							TraceEventType.Start => ConsoleColor.Cyan,
							TraceEventType.Stop => ConsoleColor.Cyan,
							TraceEventType.Suspend => ConsoleColor.Cyan,
							TraceEventType.Transfer => ConsoleColor.Cyan,
							TraceEventType.Verbose => ConsoleColor.DarkGray,
							TraceEventType.Warning => ConsoleColor.DarkYellow,
							_ => Console.ForegroundColor
						};
					}

					//Writes the associated event type of this message
					Trace.Write(eventType.ToString().ToUpper());

					if(prevColor.HasValue)
						Console.ForegroundColor = prevColor.Value;

					Trace.Write(']');
				}

				//Splits the message at every new line to prevent wrong indentation
				string[] allMessages = message.Split(new[] {"\n", "\r"}, StringSplitOptions.RemoveEmptyEntries);

				if(allMessages.Length > 1)
				{
					//Writes all messages in an indented box
					Trace.WriteLine(null);
					Trace.Indent();
				}
				else
				{
					//Simply writes a space as the message is only on line long
					Trace.Write(' ');
				}

				//Writes all message lines
				for(int i = 0; i < allMessages.Length; i++)
				{
					Trace.WriteLine(allMessages[i]);
				}

				//Unindents again, if necessary
				if(allMessages.Length > 1)
					Trace.Unindent();

				//Flushes the content, if necessary
				if(AutoFlush)
					Trace.Flush();

				//Changes the trace auto flush back to the previous value
				Trace.AutoFlush = tempFlush;
			}
		}

		/// <summary>
		/// Deletes old files in the current log folder if the size of the folder exceeds a specified amount.
		/// </summary>
		/// <param name="maxFolderSize">The maximum allowed size of the folder in bytes.</param>
		public void DeleteOldLogs(long maxFolderSize = 10485760)
		{
			if(string.IsNullOrWhiteSpace(_logFolder))
				return;

			FileInfo[] logfiles = Directory.GetFiles(_logFolder, "*.*", SearchOption.AllDirectories).Select(a => new FileInfo(a)).ToArray();
			long totalBytes = logfiles.Sum(a => a.Length);

			List<FileInfo> orderedFiles = logfiles.OrderBy(a => a.LastWriteTimeUtc).ToList();

			while(totalBytes > maxFolderSize)
			{
				FileInfo oldFile = orderedFiles.FirstOrDefault();

				totalBytes -= oldFile?.Length ?? 0;

				try
				{
					oldFile?.Delete();
				}
				catch(Exception)
				{
					//Ignore
				}
				orderedFiles.Remove(oldFile);
			}
		}

		/// <summary>
		/// Initializes this object.
		/// </summary>
		private void Initialize(bool writeToConsole, string logfolder = null, string filename = null)
		{
			UsesConsole = writeToConsole;
			UsesLogFile = !string.IsNullOrWhiteSpace(logfolder);

			_logFolder = logfolder;

			if(writeToConsole)
			{
				var listener = new TextWriterTraceListener(Console.Out);
				Trace.Listeners.Add(listener);
				_listeners.Add(listener);
			}

			if(UsesLogFile)
			{
				logfolder = logfolder.ReplaceInvalidPathChars('_');
				filename = !string.IsNullOrWhiteSpace(filename) 
					? filename.ReplaceInvalidFileChars('_') 
					: $"{DateTime.Now:yyyy-MM-dd}.log".ReplaceInvalidFileChars('_');

				if(!Directory.Exists(logfolder))
					Directory.CreateDirectory(logfolder);

				string combined = Path.Combine(logfolder, filename);

				bool fileExisted = File.Exists(combined);

				var listener = new TextWriterTraceListener(combined);

				Trace.Listeners.Add(listener);
				_listeners.Add(listener);

				if(fileExisted) 
					listener.WriteLine(null);
			}

			Start();
		}

		/// <summary>
		/// Writes a message with the name of the current <see cref="AppDomain"/>
		/// and indents the <see cref="Trace"/> afterwards.
		/// </summary>
		private void Start()
		{
			Write($"--- {AppDomain.CurrentDomain.FriendlyName} ---", TraceEventType.Start);
			Trace.Indent();
		}

		/// <summary>
		/// Unindents the <see cref="Trace"/> and writes a message with the name of the
		/// current <see cref="AppDomain"/> afterwards.
		/// </summary>
		private void End()
		{
			Trace.Unindent();
			Write($"--- {AppDomain.CurrentDomain.FriendlyName} ---", TraceEventType.Stop);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			lock(_lock)
			{
				End();

				for(int i = 0; i < _listeners.Count; i++)
				{
					_listeners[i].Dispose();
					Trace.Listeners.Remove(_listeners[i]);
				}

				if(!DeleteOnDispose)
					return;

				try
				{
					DeleteOldLogs();
				}
				catch(Exception)
				{
					//Ignore
				}
			}
		}
	}
}
