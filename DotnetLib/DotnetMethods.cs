using Librac.ProcessHandlerLib;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Librac.DotnetLib
{
    internal class DotnetMethods : IDotnet
    {
        public string[] Run_LaunchAssembly(
            IProcessHandler? processHandler,
            string assemblyPath,
            bool hideWindow = false,
            bool runAsAdmin = false,
            string infoSaveLocation = "",
            Action? callback = null)
        {
            ProcessStartInfo startInfo = CreateStartInfo(assemblyPath, hideWindow, runAsAdmin);

            var process = new Process();
            process.StartInfo = startInfo;
            process.EnableRaisingEvents = true;

            var writer = new OutputWriter((value) => Console.WriteLine(value));
            SubscribeToOutputStream(process, writer);

            process.Exited += (sender, e) =>
            {
                UnSubscribeFromOutputStream(process, writer);
                callback?.Invoke();
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

        public async Task Run_ExecuteAssemblyAsync(
            string assemblyPath,
            bool hideWindow = false,
            bool runAsAdmin = false,
            string args = "",
            Action? callback = null)
        {
            string result = string.Empty;

            ProcessStartInfo startInfo = CreateStartInfo(assemblyPath, hideWindow, runAsAdmin);

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
                if (hideWindow)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

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

        #region AUXILARY
        private static ProcessStartInfo CreateStartInfo(string assemblyPath, bool hideWindow, bool runAsAdmin)
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
            return startInfo;
        }

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

