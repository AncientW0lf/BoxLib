using BoxLib.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BoxLib.Tests
{
	[TestClass]
	public class DateTimerTest
	{
		[TestMethod]
		public void TestTiming()
		{ 
			Task.Run(TestTimingFunc).GetAwaiter().GetResult();
		}

		private static async Task TestTimingFunc()
		{
			var waiter = new AutoResetEvent(false);

			DateTime timing = DateTime.Now;
			timing = timing.Subtract(new TimeSpan(0, 0, timing.Second));
			timing = timing.Subtract(new TimeSpan(0, 0, 0, 0, timing.Millisecond));
			timing = timing.AddMinutes(1);

			using var timer = new DateTimer(10)
			{
				Interval = new TimeSpan(0, 0, 6),
				AutoReset = false,
				StartTime = timing,
				HandleStartTimeAsElapsed = false
			};
			timer.Elapsed += (sender, e) =>
			{
				waiter.Set();
			};
			timer.Start();

			Assert.IsTrue(timer.Enabled);

			await Task.Run(() =>
			{
				waiter.WaitOne(60000 * 3);
				DateTime now = DateTime.Now;
				Assert.IsTrue(timing.AddSeconds(6).ToString("T")
						.Equals(now.Subtract(new TimeSpan(0, 0, 0, 0, now.Millisecond)).ToString("T")), 
					$"Timing is off! Should be {timing.AddSeconds(6):T} but is {now:T}.");
			});
		}
	}
}