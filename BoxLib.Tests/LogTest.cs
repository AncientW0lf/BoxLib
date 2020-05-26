using System.Diagnostics;
using System.IO;
using BoxLib.Static;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxLib.Tests
{
	[TestClass]
	public class LogTest
	{
		private void ClearLogFolder()
		{
			string[] files = Directory.GetFiles("Logs");
			for(int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
		}
		
		[TestMethod]
		public void TestVerboseLoggingOn()
		{
			ClearLogFolder();

			Log.ShowVerbose = true;
			Log.Write("Test Test Test", 1, TraceEventType.Verbose);
			Log.Close();

			using FileStream file = File.OpenRead($@"Logs\{Log.LogFile}");
			using var reader = new StreamReader(file);

			Assert.IsTrue(reader.ReadToEnd().ToLower().Contains(TraceEventType.Verbose.ToString().ToLower()));
		}

		[TestMethod]
		public void TestVerboseLoggingOff()
		{
			ClearLogFolder();

			Log.ShowVerbose = false;
			Log.Write("Test Test Test", 1, TraceEventType.Verbose);
			Log.Close();

			using FileStream file = File.OpenRead($@"Logs\{Log.LogFile}");
			using var reader = new StreamReader(file);

			Assert.IsFalse(reader.ReadToEnd().ToLower().Contains(TraceEventType.Verbose.ToString().ToLower()));
		}

		[TestMethod]
		public void TestClose()
		{
			for(int i = 0; i < 5; i++)
			{
				Directory.CreateDirectory("Logs");
				using FileStream file = File.Create($@"Logs\BigFile{i}");
				using var writer = new StreamWriter(file);

				while(file.Length < 5242880)
				{
					writer.Write(1);
				}
			}

			Log.MaxBytes = 6291456;
			Log.Close();

			int fileCount = Directory.GetFiles("Logs").Length;
			Assert.IsTrue(fileCount == 1, $"{fileCount} files remain.");

			ClearLogFolder();
		}
	}
}
