using System.Diagnostics;
using System.IO;
using BoxLib.Static;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxLib.Tests
{
	[TestClass]
	public class LogTest
	{
		private static void ClearLogFolder()
		{
			string[] files = Directory.GetFiles("Logs");
			for(int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
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
