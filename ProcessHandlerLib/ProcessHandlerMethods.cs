using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Librac.ProcessHandlerLib
{
    internal class ProcessHandlerMethods : IProcessHandler
    {
        private readonly ShellCommands shellCommands = new ShellCommands();

        internal readonly int PID = 0;
        internal readonly int CREATON_TIME = 1;

        #region POWERSHELL EXECUTOR 
        private async Task<string> ExecuteInBackgroundAsync(string command, bool asAdmin)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = command,
                Verb = asAdmin ? "runAs" : string.Empty,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            StringBuilder sb = new StringBuilder();

            using (Process process = new Process())
            {
                process.StartInfo = info;
                process.OutputDataReceived += (sender, args) => 
                { 
                    if (args.Data != null) 
                        sb.AppendLine(args.Data); 
                };
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        sb.AppendLine(args.Data);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await WaitForExitAsync(process);
            }

            return sb.ToString();
        }

        private Task WaitForExitAsync(Process process)
        {
            var tcs = new TaskCompletionSource<object>();
            process.Exited += (sender, args) =>
            {
                tcs.SetResult(true);
            };
            process.EnableRaisingEvents = true;
            return tcs.Task;
        }
        #endregion

        #region PROCESS INFO SAVING
        public string SaveProcessInfo(Process process, string fullFileName)
        {
            if (string.IsNullOrEmpty(fullFileName))
                return "Full file name not provided";

            var startTime = process.StartTime.ToString();
            try
            {
                File.WriteAllText(fullFileName, $"{process.Id}|{startTime}");
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Process PID and TimeCreated saved succesfully";
        }

        private static string[] LoadProcessInfo(string fileLocation)
        {
            try
            {
                return File.ReadAllText(fileLocation).Split("|");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Problem with reading the file: {e}");
            }
            return new string[] { };
        }
        #endregion

        #region PROCESS KILLING
        public string Kill_Process_ByPIDAndTimeCreated(string fullFileName)
        {
            if (!File.Exists(fullFileName))
                return string.Empty;

            var info = LoadProcessInfo(fullFileName);

            string result = "";

            foreach (var process in Process.GetProcesses())
            {
                if (process.Id.ToString() == info[PID])
                {
                    // Check if the process has the same metadata
                    if (process.StartTime.ToString() == info[CREATON_TIME])
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                                result += $"Process with 'PID: {PID}' terminated" + Environment.NewLine;
                            }
                        }
                        catch (Exception e)
                        {
                            result += $"Failed to kill the process: {e.Message}" + Environment.NewLine;
                        }
                        finally
                        {
                            process.Dispose();
                        }
                        File.Delete(fullFileName);
                        break;
                    }
                }
            }
            return result;
        }

        public string Kill_Process_ByName_FastUnsafe(params string[] args)
        {
            var command = shellCommands.Get_KillProcesesByName_FastUnsafe(args);
            return Task.Run(() => ExecuteInBackgroundAsync(command, true)).Result;
        }

        public string Kill_Process_ByName(bool limitScopeToCurrentUser = true, params string[] args)
        {
            var command = shellCommands.Get_KillProcesesByName(limitScopeToCurrentUser, args);
            return Task.Run(() => ExecuteInBackgroundAsync(command, true)).Result;
        }

        public string Kill_DotnetProcess_ByFullNameFilter(string filter)
        {
            var command = shellCommands.Get_KillDotnetProcessByFullProcessNameFilter(filter);
            return Task.Run(() => ExecuteInBackgroundAsync(command, true)).Result;
        }

        public string Kill_Process_ByFullNameFilter(string filter, bool limitScopeToCurrentUser = true)
        {
            var command = shellCommands.Get_KillProcessByFullNameFilter(filter, limitScopeToCurrentUser);
            return Task.Run(() => ExecuteInBackgroundAsync(command, true)).Result;
        }

        public string Kill_Process_ByTcpPortListened(params int[] ports)
        {
            var command = shellCommands.Get_KillByTcpPorts(ports);
            return Task.Run(() => ExecuteInBackgroundAsync(command, true)).Result;
        }
        #endregion

        #region AUXILIARY
        internal async Task FindProcesesByWindowStyle()
        {
            var command = "Get-Process | Where-Object { $_.MainWindowTitle -ne \"\" } | Select-Object Id, Name, MainWindowTitle";
            await ExecuteInBackgroundAsync(command, true);
        }
        #endregion
    }

}
