using BoxLib.Static;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BoxLib.Tests
{
	[TestClass]
	public class MemoryTest
	{
		[TestMethod]
		public void TestMemoryReduction()
		{
			var currProc = Process.GetCurrentProcess();

			long currMemory = currProc.WorkingSet64;
			Memory.MinimizeFootprint();
			currProc.Refresh();
			long newMemory = currProc.WorkingSet64;

			Assert.IsTrue(newMemory < currMemory, "Memory hasn't been reduced! " +
			                                      $"{newMemory} should be smaller than {currMemory}.");
		}
	}
}
