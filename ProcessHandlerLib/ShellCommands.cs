using System.Linq;
using System.Text;

namespace Librac.ProcessHandlerLib
{
    internal class ShellCommands
    {
        internal string Get_KillByTcpPortsScript(params int[] ports)
        {
            string portLine = $"$ports = @({string.Join(", ", ports)});";

            return portLine + @"
            $processes = Get-NetTCPConnection | Where-Object { $ports -contains $_.LocalPort };
            foreach ($proc in $processes) {
                 $processDetail = Get-Process -Id $proc.OwningProcess;
                 Write-Host \""Stopping process $($processDetail.Name) on port $($proc.LocalPort)\"";
                 $processDetail | Stop-Process -Force;
            }";
        }

        internal string Get_KillProcessByOwnerNameCommand(string processName)
        {
            string command = @"
            $processes = Get-WmiObject Win32_Process -Filter ""Name = 'dotnet.exe'""
            foreach ($process in $processes) {
            $ownerInfo = $process.GetOwner().User
                if ($process.Commandline -and $process.Commandline -like '*" + processName + @"*') {
                    Write-Host ""Terminating process $($process.Name) with PID $($process.ProcessId) and Owner $ownerInfo""
                    try {
                        $proc = Get-Process -Id $process.ProcessId
                        $proc.Kill()
                        Write-Host 'Process terminated successfully.'
                    } catch {
                        Write-Host ""Failed to terminate process: $_""
                    }
                }
            }";
            return command.Replace("\"", "\\\"");
        }

        /// <summary>
        /// Generates a PowerShell command to forcefully terminate processes by name, allowing for inclusion (name matches) and exclusion (name does not match prefixed with "!") criteria.
        /// <para>
        /// Example of arguments: ("test", "!production") will generate a script that kills all processes that contain "test" but do not contain "production"
        /// </para>
        /// </summary> 
        internal string Get_DeleteProcesesByNameCommand(params string[] args)
        {
            var like = args.Where(x => !x.StartsWith("!")).Distinct().ToArray();
            var notLike = args.Where(x => x.StartsWith("!")).Distinct().ToArray();

            var builder = new StringBuilder();
            builder.Append("Get-Process | Where-Object { (");
            for (int i = 0; i < like.Length; i++)
            {
                builder.Append($"$_.ProcessName -like '*{like[i]}*'");
                if (i < like.Length - 1)
                    builder.Append(" -or ");
            }
            if (notLike.Any())
                builder.Append(") -and (");
            for (int i = 0; i < notLike.Length; i++)
            {

                builder.Append($"$_.ProcessName -notlike '*{notLike[i].Replace("!", string.Empty)}*'");
                if (i < notLike.Length - 1)
                    builder.Append(" -and ");
            }

            builder.Append(") } | Stop-Process -Force\r\n");
            return builder.ToString();
        }

        //internal string Get_DeleteProcesesByNameCommand2(params string[] args)
        //{

        //}
    }
}
