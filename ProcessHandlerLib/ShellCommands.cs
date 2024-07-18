using System.Linq;
using System.Text;

namespace Librac.ProcessHandlerLib
{
    internal class ShellCommands
    {
        private readonly string ExecutionPolicySafetycheck = "Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process ";
        internal string Get_KillByTcpPorts(params int[] ports)
        {
            string portLine = $"$ports = @({string.Join(", ", ports)});";

            return $@"
            {ExecutionPolicySafetycheck}
            {portLine}
            $found = $false
            $processes = Get-NetTCPConnection | Where-Object {{ $ports -contains $_.LocalPort }};
            foreach ($proc in $processes) {{
            try {{
                $processDetail = Get-Process -Id $proc.OwningProcess;
                if ($processDetail) {{
                    $processDetail | Stop-Process -Force;
                    Write-Host 'Process terminated successfully.'
                }} else {{
                    Write-Host ""Process ID $($proc.OwningProcess) not found""
                }}
                $found = $true
                }} catch {{
                    Write-Host ""Failed to terminate process ID $($proc.OwningProcess): $($_.Exception.Message)""
                }}
            }}
            if (-not $found) {{
                Write-Host 'No processes found on the specified ports to terminate.'
            }}";
        }

        internal string Get_KillDotnetProcessByFullProcessNameFilter(string processName)
        {
            string command = $@"
            {ExecutionPolicySafetycheck}
            try {{
                $processes = Get-WmiObject Win32_Process -Filter ""Name = 'dotnet.exe'""
                $found = $false
                foreach ($process in $processes) {{
                $ownerInfo = $process.GetOwner().User
                    if ($process.CommandLine -and $process.CommandLine -like '*{processName}*') {{
                        $found = $true
                        try {{
                            $proc = Get-Process -Id $process.ProcessId
                            $proc.Kill()
                             Write-Host ""Process with id $process.ProcessId terminated successfully.""
                        }} catch {{
                            Write-Host ""Failed to terminate process: $($_.Exception.Message)""
                        }}
                    }}
                }}
                if (-not $found) {{
                    Write-Host 'No matching processes found to terminate.'
                }}
            }} catch {{
                Write-Host ""Failed to execute script: $($_.Exception.Message)""
            }}";
            return command.Replace("\"", "\\\"");
        }

        internal string Get_KillProcessByFullNameFilter(string processName, bool limitScopeToCurrentUser = true)
        {
            if (limitScopeToCurrentUser)
                return Get_KillCurrentUserProcessByFullNameFilter(processName);
            else
                return Get_KillAnyProcessByFullNameFilter(processName);
        }

        private string Get_KillCurrentUserProcessByFullNameFilter(string processName)
        {
            string command = $@"
            {ExecutionPolicySafetycheck}
            try {{
                $currentUser = $env:USERNAME
                $processes = Get-WmiObject Win32_Process | Where-Object {{ $_.CommandLine -and $_.CommandLine -like '*{processName}*' }}
                $found = $false
                foreach ($process in $processes) {{
                    $ownerInfo = $process.GetOwner().User
                    if ($ownerInfo -eq $currentUser) {{
                        $found = $true
                        try {{
                            $proc = Get-Process -Id $process.ProcessId
                            $proc.Kill()
                             Write-Host ""Process with id $process.ProcessId terminated successfully.""
                        }} catch {{
                            Write-Host ""Failed to terminate process: $($_.Exception.Message)""
                        }}
                    }}
                }}
                if (-not $found) {{
                    Write-Host 'No matching processes found to terminate for the current user.'
                }}
            }} catch {{
                Write-Host ""Failed to execute script: $($_.Exception.Message)""
            }}";
            return command.Replace("\"", "\\\"");
        }

        private string Get_KillAnyProcessByFullNameFilter(string processName)
        {
            string command = $@"
            {ExecutionPolicySafetycheck}
            try {{
                $processes = Get-WmiObject Win32_Process | Where-Object {{ $_.CommandLine -and $_.CommandLine -like '*{processName}*' }}
                $found = $false
                foreach ($process in $processes) {{
                    $found = $true
                    try {{
                        $proc = Get-Process -Id $process.ProcessId
                        $proc.Kill()
                        Write-Host ""Process with id $process.ProcessId terminated successfully.""
                    }} catch {{
                        Write-Host ""Failed to terminate process: $($_.Exception.Message)""
                    }}
                }}
                if (-not $found) {{
                    Write-Host 'No matching processes found to terminate.'
                }}
            }} catch {{
                Write-Host ""Failed to execute script: $($_.Exception.Message)""
            }}";
            return command.Replace("\"", "\\\"");
        }


        /// <summary>
        /// Generates a PowerShell command to forcefully terminate processes by name, allowing for inclusion (name matches) and exclusion (name does not match prefixed with "!") criteria.
        /// <para>
        /// Example of arguments: ("test", "!production") will generate a script that kills all processes that contain "test" but do not contain "production"
        /// </para>
        /// </summary> 
        internal string Get_KillProcesesByName(params string[] args)
        {
            var like = args.Where(x => !x.StartsWith("!")).Distinct().ToArray();
            var notLike = args.Where(x => x.StartsWith("!")).Distinct().ToArray();

            var builder = new StringBuilder();
            builder.AppendLine(ExecutionPolicySafetycheck);
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
    }
}
