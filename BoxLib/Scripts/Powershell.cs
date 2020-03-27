using System.Diagnostics;
using System.Threading.Tasks;

namespace BoxLib.Scripts
{
	public static class PowerShell
	{
		public static bool UseLog = true;

		public static async Task Run(string command, int timeout)
		{
			if(UseLog)
				Log.Write("Starting new PowerShell process...", 1, TraceEventType.Information);

			using var proc = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					RedirectStandardError = true,
					RedirectStandardOutput = true,
					CreateNoWindow = true,
					ErrorDialog = true,
					FileName = "Powershell.exe",
					Arguments = $"-NoLogo -NonInteractive -OutputFormat Text -Command {command}",
					UseShellExecute = false
				}
			};
			proc.Start();
			await Task.Run(() => proc.WaitForExit(timeout));

			if(!proc.HasExited)
				proc.Kill();

			if(UseLog)
			    Log.Write($"Closed PowerShell process with exit code {proc.ExitCode}.", 1, TraceEventType.Information);

			string outp = proc.StandardOutput.ReadToEnd();
			string errp = proc.StandardError.ReadToEnd();

            if(UseLog)
            {
                Log.Write("PowerShell output:", 1, TraceEventType.Information, true);
                Log.Write($"{outp}\nErrors:\n{errp}", 2, null);
            }

			proc.Close();
		}

		public static async Task SetIP(string alias, string newAddress)
		{
			await Run($"Set-NetIPAddress -InterfaceAlias '{alias}' -IPAddress '{newAddress}'", 30000);
		}
	}
}