using System;
using System.Runtime.InteropServices;

namespace BoxLib.External
{
	public static class ConsoleWinHelper
	{
		private const int SW_HIDE = 0;
		private const int SW_SHOW = 5;

		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		public static void HideWindow()
		{
			IntPtr handle = GetConsoleWindow();

			// Hide
			ShowWindow(handle, SW_HIDE);
		}

		public static void ShowWindow()
		{
			IntPtr handle = GetConsoleWindow();

			// Show
			ShowWindow(handle, SW_SHOW);
		}
	}
}
