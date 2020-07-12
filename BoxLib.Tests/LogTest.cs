using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using BoxLib.Objects;

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
		public void TestCleanup()
		{
			var log = new Log(false, "Logs");

			Directory.CreateDirectory("Logs");
			using(FileStream file = File.Create(@"Logs\BigFile0"))
			{
				using var writer = new StreamWriter(file);

				while(file.Length < 5242880)
				{
					writer.Write(1);
				}
			}

			for(int i = 1; i < 5; i++)
			{
				string currFile = $@"Logs\BigFile{i}";

				if(File.Exists(currFile))
					File.Delete(currFile);

				File.Copy(@"Logs\BigFile0", currFile);
			}

			log.DeleteOldLogs(5242880);
			log.Dispose();

			int fileCount = Directory.GetFiles("Logs").Length;
			Assert.IsTrue(fileCount == 1, $"{fileCount} files remain.");

			ClearLogFolder();
		}
	}
}
