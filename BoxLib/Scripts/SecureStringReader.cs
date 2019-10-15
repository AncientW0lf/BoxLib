using System;
using System.Runtime.InteropServices;
using System.Security;

namespace BoxLib.Scripts
{
	public static class SecureStringReader
	{
		/// <summary>
		/// Creates a temporary environment to handle a <see cref="SecureString"/> to prevent exposure of sensitive data.
		/// </summary>
		/// <param name="data">The sensitive data that will be read from memory.</param>
		/// <param name="handler">The method to execute after the <see cref="string"/> was found in memory.
		/// The argument is the underlying <see cref="string"/> object of the <see cref="data"/>.</param>
		public static void Handle(SecureString data, Action<string> handler) {
			IntPtr valuePtr = IntPtr.Zero;
			try {
				valuePtr = Marshal.SecureStringToGlobalAllocUnicode(data);
				handler(Marshal.PtrToStringUni(valuePtr));
			} finally {
				Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}
	}
}
