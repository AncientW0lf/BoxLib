using System.IO;
using BoxLib.Static;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BoxLib.Tests
{
	[TestClass]
	public class LogTest
	{
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

			string[] files = Directory.GetFiles("Logs");
			Assert.IsTrue(files.Length == 1, $"{files.Length} files remain.");

			for(int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
		}
	}
}
