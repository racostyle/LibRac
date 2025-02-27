using LibRac.Shell.Logger;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibRac.Shell
{
    /// <summary>
    /// Class for running async scripts in PowerShell
    /// </summary>
    public class ShellExecutor
    {
        private readonly IShellLogger _logger;

        /// <summary>
        /// 
        /// </summary>
        public ShellExecutor()
        {
            _logger = new VoidLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        public ShellExecutor(StringBuilder stringBuilder)
        {
            _logger = new StringBuilderLogger(stringBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        public ShellExecutor(Action<string> action)
        {
            _logger = new ActionLogger(action);
        }

        /// <summary>
        /// Executes a PowerShell command and returns its output.
        /// </summary>
        /// <param name="command">The PowerShell command to execute.</param>
        /// <param name="workingDir">The working directory in which to run the command.</param>
        /// <param name="visible">Determines whether the process should be visible (default: true).</param>
        /// <param name="asAdmin">Specifies whether the process should be run with administrative privileges (default: true).</param>
        /// <param name="timeoutInMills">The timeout in milliseconds before forcefully terminating the process (-1 for infinite wait).</param>
        /// <returns>A task that with command output as a string.</returns>
        public async Task ExecuteAsync(string command, string workingDir, bool visible = true, bool asAdmin = true, int timeoutInMills = -1)
        {
            await Execute(command, workingDir, visible, asAdmin, timeoutInMills);
        }

        /// <summary>
        /// Executes a PowerShell command as a background job and waits for completion.
        /// </summary>
        /// <param name="command">The PowerShell command to execute.</param>
        /// <param name="workingDir">The working directory in which to run the command.</param>
        /// <param name="visible">Determines whether the process should be visible (default: true).</param>
        /// <param name="asAdmin">Specifies whether the process should be run with administrative privileges (default: true).</param>
        /// <param name="timeoutInMills">The timeout in milliseconds before forcefully terminating the process (-1 for infinite wait).</param>
        /// <returns>A task that with command output as a string.</returns>
        public async Task ExecuteAsAJobAsync(string command, string workingDir, bool visible = true, bool asAdmin = true, int timeoutInMills = -1)
        {
            command = WrapCommandInJob(command);
            await Execute(command, workingDir, visible, asAdmin, timeoutInMills);
        }

        #region Auxiliary
        private static string WrapCommandInJob(string command)
        {
            command = $@"
                $nadgradnja = Start-Job -ScriptBlock {{
                    {command}
                }}
                Wait-Job $nadgradnja
                $output = Receive-Job -Job $nadgradnja
                Remove-Job -Job $nadgradnja
                Write-Output $output";
            return command;
        }
        #endregion

        #region EXECUTOR
        private async Task Execute(string command, string workingDir, bool visible, bool asAdmin, int timeoutInMills)
        {
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                Verb = asAdmin ? "runAs" : string.Empty,
                UseShellExecute = visible,
                RedirectStandardOutput = !visible,
                RedirectStandardError = !visible,
                CreateNoWindow = !visible,
                WorkingDirectory = workingDir
            };

            using var process = new Process() { StartInfo = info };

            try
            {
                StartProcess(process, !visible);

                if (timeoutInMills == Timeout.Infinite)
                {
                    await WaitForExitAsync(process); // No timeout, wait indefinitely
                }
                else
                {
                    var processExitTask = WaitForExitAsync(process); ;
                    using var cts = new CancellationTokenSource();
                    var timeoutTask = Task.Delay(timeoutInMills, cts.Token);

                    var completedTask = await Task.WhenAny(processExitTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        process?.Kill();
                        _logger.Log($"Process timed out after {timeoutInMills} ms and was terminated.");
                    }
                    else
                        cts.Cancel();
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Error: {ex.Message}");
            }
        }

        private void StartProcess(Process process, bool captureOutput)
        {
            if (captureOutput)
            {
                process.OutputDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        _logger.Log(args.Data);
                };
                process.ErrorDataReceived += (sender, args) =>
                {
                    if (args.Data != null)
                        _logger.Log(args.Data);
                };
            }

            try
            {
                if (captureOutput)
                {
                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                }
                else
                {
                    process.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed to start process: {ex.Message}");
            }
        }

        private Task WaitForExitAsync(Process process)
        {
            var tcs = new TaskCompletionSource<object>();
            process.EnableRaisingEvents = true;

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(true);
            };

            return tcs.Task;
        }
        #endregion
    }
}
