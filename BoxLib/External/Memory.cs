using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace BoxLib.External
{
	/// <summary>
	/// Provides methods for direct memory manipulation of your application.
	/// </summary>
	public static class Memory
	{
		[DllImport("psapi.dll")]
		private static extern int EmptyWorkingSet(IntPtr hwProc);

		/// <summary>
		/// Reduces the working set of your application to a minimum.
		/// This causes hard page faults if the memory is needed afterwards.
		/// Only works on Windows.
		/// </summary>
		public static void MinimizeFootprint()
		{
			EmptyWorkingSet(Process.GetCurrentProcess().Handle);
		}
	}
}
