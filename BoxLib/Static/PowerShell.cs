using System.Diagnostics;
using System.Threading.Tasks;
using BoxLib.Objects;

namespace BoxLib.Static
{
	public static class PowerShell
	{
		public static async Task Run(string command, int timeout, Log log = null)
		{
			log?.Write("Starting new PowerShell process...", TraceEventType.Information);

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

			log?.Write($"Closed PowerShell process with exit code {proc.ExitCode}.", TraceEventType.Information);

			string outp = proc.StandardOutput.ReadToEnd();
			string errp = proc.StandardError.ReadToEnd();

            log?.Write("PowerShell output:\n" + 
                       $"{outp}\nErrors:\n{errp}", TraceEventType.Information);

			proc.Close();
		}

		public static async Task SetIP(string alias, string newAddress)
		{
			await Run($"Set-NetIPAddress -InterfaceAlias '{alias}' -IPAddress '{newAddress}'", 30000);
		}
	}
}