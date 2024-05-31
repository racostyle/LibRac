using Librac.ProcessHandlerLib;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Librac.DotnetLib
{
    internal class DotnetMethods : IDotnet
    {
        #region EXTERNAL PROCESS
        public string[] Run_LaunchAssembly(
            IProcessHandler? processHandler,
            string assemblyPath,
            bool hideWindow = false,
            bool runAsAdmin = false,
            string infoSaveLocation = "",
            Action<string>? outputCallback = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = assemblyPath,
                UseShellExecute = !hideWindow, //if hideWindow = true it must be false to redirect output
                CreateNoWindow = hideWindow,
                RedirectStandardOutput = hideWindow,
                RedirectStandardError = hideWindow,
                Verb = runAsAdmin ? "runAs" : string.Empty,
            };

            var process = new Process();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            OutputWriter? writer = null;
            if (outputCallback != null)
            {
                writer = new OutputWriter(outputCallback);
                SubscribeToOutputStream(process, writer);
            }

            process.Exited += (sender, e) =>
            {
                UnSubscribeFromOutputStream(process, writer);
                process.Dispose();
            };

            process.Start();

            if (processHandler != null)
                processHandler.SaveProcessInfo(process, infoSaveLocation);

            if (hideWindow)
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            return new string[] { process.Id.ToString(), process.StartTime.ToString() };
        }
        #endregion

        #region INTERNAL PROCESS
        public async Task Run_ExecuteAssemblyAsync(string assemblyPath, string args = "", Action? callback = null)
        {
            string result = string.Empty;

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dotnet", // Use the dotnet runtime
                Arguments = $"{assemblyPath} {args}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runAs"
            };

            var tcs = new TaskCompletionSource<bool>();
            using (var process = new Process())
            {
                process.StartInfo = startInfo;
                process.EnableRaisingEvents = true;
                var writer = new OutputWriter((value) => Console.WriteLine(value));
                SubscribeToOutputStream(process, writer);

                process.Exited += (sender, e) =>
                {
                    UnSubscribeFromOutputStream(process, writer);
                    callback?.Invoke();
                    process.Dispose();
                    tcs.SetResult(false);
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Wait for the process to exit
                await tcs.Task;
            }
        }

        public Process Run_LaunchAssemblySimple(string assemblyPath, string args = "", Action? callback = null)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dotnet", // Use the dotnet runtime
                Arguments = $"{assemblyPath} {args}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                Verb = "runAs"
            };

            var process = new Process();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            var writer = new OutputWriter((value) => Console.WriteLine(value));
            SubscribeToOutputStream(process, writer);

            process.Exited += (sender, e) =>
            {
                UnSubscribeFromOutputStream(process, writer);
                callback?.Invoke();
                // Do not close or dispose of the process here, as we want to return it to the caller.
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            return process;
        }
        #endregion

        #region AUXILARY
        private void SubscribeToOutputStream(Process process, OutputWriter? writer)
        {
            if (writer != null)
            {
                process.OutputDataReceived += writer.OutputDataRecieved;
                process.ErrorDataReceived += writer.ErrorDataRecieved;
            }
        }

        private void UnSubscribeFromOutputStream(Process process, OutputWriter? writer)
        {
            if (writer != null)
            {
                process.OutputDataReceived -= writer.OutputDataRecieved;
                process.ErrorDataReceived -= writer.ErrorDataRecieved;
            }
        }
        #endregion
    }
}

