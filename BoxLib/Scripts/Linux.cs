using System.ComponentModel;
using System.Diagnostics;

namespace BoxLib.Scripts
{
	public static class Linux
	{
		public static string Bash(string command)
		{
			string escapedArgs = command.Replace("\"", "\\\"");
            
			using var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "/bin/bash",
					Arguments = $"-c \"{escapedArgs}\"",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				}
			};
			try
			{
				process.Start();
			}
			catch(Win32Exception e)
			{
				return e.Message;
			}
			process.WaitForExit(60000);
			string result = process.StandardOutput.ReadToEnd();
			return result;
		}
	}
}
