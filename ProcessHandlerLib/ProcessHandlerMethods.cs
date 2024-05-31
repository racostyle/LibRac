using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librac.ProcessHandlerLib
{
    internal class ProcessHandlerMethods : IProcessHandler
    {
        internal readonly int PID = 0;
        internal readonly int CREATON_TIME = 1;

        #region KILL PROCESS BY PID AND CRATE TIME
        public void KillProcess_ByPIDAndTimeCreated(string fullFileName)
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
        #endregion

        #region KILL PROCESS BY NAME
        public async Task KillProcess_ByName(params string[] args)
        {
            var command = GenerateDeleteProcesesByNameCommand(args);
            await ExecuteInBackgroundAsync(command, true);
        }

        private async Task ExecuteInBackgroundAsync(string command, bool asAdmin)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = command,
                Verb = asAdmin ? "runAs" : string.Empty,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = info;
                await Task.Run(() =>
                {
                    process.Start();
                    process.WaitForExit();
                });
            }
        }

        /// <summary>
        /// Generates a PowerShell command to forcefully terminate processes by name, allowing for inclusion (name matches) and exclusion (name does not match prefixed with "!") criteria.
        /// <para>
        /// Example of arguments: ("test", "!production") will generate a script that kills all processes that contain "test" but do not contain "production"
        /// </para>
        /// </summary> 
        private string GenerateDeleteProcesesByNameCommand(params string[] args)
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
                    builder.Append(" -or ");
            }

            builder.Append(") } | Stop-Process -Force\r\n");
            return builder.ToString();
        }
        #endregion

        internal async Task FindProcesesByWindowStyle()
        {
            var command = "Get-Process | Where-Object { $_.MainWindowTitle -ne \"\" } | Select-Object Id, Name, MainWindowTitle";
            await ExecuteInBackgroundAsync(command, true);
        }
    }

}
