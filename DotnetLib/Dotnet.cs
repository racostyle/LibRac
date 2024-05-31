using Librac.ProcessHandlerLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Librac.DotnetLib
{
    public static class Dotnet
    {
        private static readonly IDotnet _dotnet = new DotnetMethods();

        /// <summary>
        /// Asynchronously executes a .NET assembly, automatically manages the process lifecycle, captures output and error streams, and invokes 
        /// a callback upon completion.
        /// </summary>
        /// <param name="assemblyPath">The file path of the .NET assembly to execute.</param>
        /// <param name="args">Optional arguments to pass to the assembly. Defaults to an empty string if no arguments are provided.</param>
        /// <param name="callback">An optional callback action that is invoked when the process exits. Can be <c>null</c>, in which case no action is taken on</param>
        /// <returns>A task that represents the asynchronous operation of running the .NET assembly and awaiting its completion.</returns>
        public static async Task Run_ExecuteAssemblyAsync(string assemblyPath, string args = "", Action? callback = null)
        {
            await _dotnet.Run_ExecuteAssemblyAsync(assemblyPath, args, callback);
        }

        /// <summary>
        /// Starts a .NET assembly as an ongoing process, returns the Process object for internal management, captures output and error streams, 
        /// and allows for an optional callback on process exit.
        /// </summary>
        /// <param name="assemblyPath">The path to the .NET assembly (.dll or .exe) to be executed.</param>
        /// <param name="args">Optional arguments to pass to the assembly. Defaults to an empty string if no arguments are provided.</param>
        /// <param name="callback">An optional callback action that is invoked when the process exits. Can be <c>null</c>, in which case no action is taken on 
        /// process exit.</param>
        /// <returns>The started <see cref="Process"/> object, allowing the caller to control and monitor the process's execution and lifecycle.</returns>
        public static Process Run_LaunchAssemblySimple(string assemblyPath, string args = "", Action? callback = null)
        {
            return _dotnet.Run_LaunchAssemblySimple(assemblyPath, args, callback);
        }

        /// <summary>
        /// Initiates a .NET assembly as a separate process with configurable window visibility and administrator privileges, returns process 
        /// ID and start time, supports asynchronous output/error stream capture, and ensures automatic process cleanup.
        /// </summary>
        /// <param name="assemblyPath">The path to the .NET assembly (.dll or .exe) to be executed.</param>
        /// <param name="hideWindow">Determines whether the window of the separate process should be hidden. Set to <c>false</c> to show the window and <c>true</c> to hide it, enabling asynchronous capture of output and error streams.</param>
        /// <param name="runAsAdmin">Specifies whether the process should be run with administrator privileges. Set to <c>true</c> to enable administrator mode.</param>
        /// <param name="infoSaveLocation">Optional. The file path where the process information (e.g., process ID and start time) will be saved. If left empty, the information is not saved.</param>
        /// <param name="outputCallback">Optional. An action delegate that receives output from the process's standard output and error streams. This parameter is only effective when <paramref name="hideWindow"/> is set to <c>true</c>.</param>
        /// <returns>An array containing the process ID and start time as strings. The first element is the process ID, and the second element is the start time.</returns>
        public static string[] Run_LaunchAssembly(string assemblyPath, bool hideWindow = false, bool runAsAdmin = false, string infoSaveLocation = "", Action<string>? outputCallback = null)
        {
            IProcessHandler? processHandler;
            if (string.IsNullOrEmpty(infoSaveLocation))
                processHandler = null;
            else
                processHandler = new ProcessHandlerMethods();
            return _dotnet.Run_LaunchAssembly(processHandler, assemblyPath, hideWindow, runAsAdmin, infoSaveLocation, outputCallback);
        }
    }
}
