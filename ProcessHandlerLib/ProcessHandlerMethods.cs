using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Librac.ProcessHandlerLib
{
    internal class ProcessHandlerMethods : IProcessHandler
    {
        private readonly ShellCommands shellCommands = new ShellCommands();

        internal readonly int PID = 0;
        internal readonly int CREATON_TIME = 1;

        #region POWERSHELL EXECUTOR 
        private async Task ExecuteInBackgroundAsync(string command, bool asAdmin)
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

            using (Process process = new Process())
            {
                process.StartInfo = info;
                process.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data);
                process.ErrorDataReceived += (sender, args) => Console.WriteLine(args.Data);

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await WaitForExitAsync(process);
            }
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

        public void Kill_Process_ByPIDAndTimeCreated(string fullFileName)
        {
            if (!File.Exists(fullFileName))
                return;

            var info = LoadProcessInfo(fullFileName);

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
                                Console.WriteLine("Process terminated!");
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Failed to kill the process: {e.Message}");
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
        }

        public void SaveProcessInfo(Process process, string fullFileName)
        {
            if (string.IsNullOrEmpty(fullFileName))
                return;

            var startTime = process.StartTime.ToString();
            File.WriteAllText(fullFileName, $"{process.Id}|{startTime}");
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

        public void Kill_Process_ByName(params string[] args)
        {
            var command = shellCommands.Get_KillProcesesByName(args);
            Task.Run(() => ExecuteInBackgroundAsync(command, true)).Wait();
        }

        public void Kill_DotnetProcess_ByFullNameFilter(string filter)
        {
            var command = shellCommands.Get_KillDotnetProcessByFullProcessNameFilter(filter);
            Task.Run(() => ExecuteInBackgroundAsync(command, true)).Wait();
        }

        public void Kill_CurrentUserProcess_ByFullNameFilter(string filter)
        {
            var command = shellCommands.Get_KillCurrentUserProcessByFullProcessNameFilter(filter);
            Task.Run(() => ExecuteInBackgroundAsync(command, true)).Wait();
        }

        public void Kill_Process_ByTcpPortListened(params int[] ports)
        {
            var script = shellCommands.Get_KillByTcpPorts(ports);
            Task.Run(() => ExecuteInBackgroundAsync(script, true)).Wait();
        }

        #region AUXILIARY
        internal async Task FindProcesesByWindowStyle()
        {
            var command = "Get-Process | Where-Object { $_.MainWindowTitle -ne \"\" } | Select-Object Id, Name, MainWindowTitle";
            await ExecuteInBackgroundAsync(command, true);
        }
        #endregion
    }

}
