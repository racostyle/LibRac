﻿using LibRac.Processes;
using System;
using System.Threading.Tasks;

namespace LibRac.DotNet
{
    /// <summary>
    /// Class for executing dll assembly
    /// </summary>
    public static class DotNetHandler
    {
        private static readonly DotNetMethods _dotNet = new DotNetMethods();
        /// <summary>
        /// Asynchronously executes an assembly with specified arguments and configuration, and invokes a callback upon completion.
        /// </summary>
        /// <param name="assemblyPath">The file path of the assembly to execute.</param>
        /// <param name="args">Optional arguments to pass to the assembly. Defaults to an empty string if no arguments are provided.</param>
        /// <param name="hideWindow">If set to true, the execution window is hidden; otherwise, it's visible.</param>
        /// <param name="runAsAdmin">If set to true, the assembly is executed with administrative privileges.</param>
        /// <param name="callback">An optional callback action to invoke when the process exits.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// The method sets up a process to run the specified assembly according to the given parameters and manages process output and error streams if <paramref name="hideWindow"/> is true.
        /// The process is disposed after execution completes, and the specified callback, if provided, is invoked.
        /// This method handles process output asynchronously and uses a TaskCompletionSource to await the process exit.
        /// </remarks>
        public static async Task Run_ExecuteAssemblyAsync(string assemblyPath, string args = "",bool hideWindow = false, bool runAsAdmin = false, Action? callback = null)
        {
            await _dotNet.Run_ExecuteAssemblyAsync(assemblyPath, args, hideWindow, runAsAdmin, callback);
        }

        /// <summary>
        /// Starts a .NET assembly as an ongoing process, returns the Processes object for internal management, captures output and error streams, 
        /// and allows for an optional callback on process exit.
        /// </summary>
        /// <param name="assemblyPath">The path to the .NET assembly (.dll or .exe) to be executed.</param>
        /// <param name="args">Optional arguments to pass to the assembly. Defaults to an empty string if no arguments are provided.</param>
        /// <param name="callback">An optional callback action that is invoked when the process exits. Can be <c>null</c>, in which case no action is taken on 
        /// process exit.</param>
        /// <returns>The started <see cref="System.Diagnostics.Process"/> object, allowing the caller to control and monitor the process's execution and lifecycle.</returns>
        public static System.Diagnostics.Process Run_LaunchAssemblySimple(string assemblyPath, string args = "", Action? callback = null)
        {
            return _dotNet.Run_LaunchAssemblySimple(assemblyPath, args, callback);
        }

        /// <summary>
        /// Initiates a .NET assembly as a separate process with configurable window visibility and administrator privileges, returns process 
        /// ID and start time, supports asynchronous output/error stream capture, and ensures automatic process cleanup.
        /// </summary>
        /// <param name="assemblyPath">The path to the .NET assembly (.dll or .exe) to be executed.</param>
        /// <param name="args">Optional arguments to pass to the assembly. Defaults to an empty string if no arguments are provided.</param>
        /// <param name="hideWindow">Determines whether the window of the separate process should be hidden. Set to <c>false</c> to show the window and <c>true</c> to hide it, enabling asynchronous capture of output and error streams.</param>
        /// <param name="runAsAdmin">Specifies whether the process should be run with administrator privileges. Set to <c>true</c> to enable administrator mode.</param>
        /// <param name="infoSaveLocation">Optional. The file path where the process information (e.g., process ID and start time) will be saved. If left empty, the information is not saved.</param>
        /// <param name="callback">An optional callback action to invoke when the process exits.</param>
        /// <returns>An array containing the process ID and start time as strings. The first element is the process ID, and the second element is the start time.</returns>
        public static string[] Run_LaunchAssembly(string assemblyPath, string args = "", bool hideWindow = false, bool runAsAdmin = false, string infoSaveLocation = "", Action? callback = null)
        {
            ProcessHandlerMethods? processHandler;
            if (string.IsNullOrEmpty(infoSaveLocation))
                processHandler = null;
            else
                processHandler = new ProcessHandlerMethods();
            return _dotNet.Run_LaunchAssembly(processHandler, assemblyPath, args, hideWindow, runAsAdmin, infoSaveLocation, callback);
        }
    }
}
