using System.Diagnostics;

namespace Librac.ProcessHandlerLib
{
    /// <summary>
    /// Class for closing procesess
    /// </summary>
    public static class ProcessHandler
    {
        private static readonly IProcessHandler _processHandler = new ProcessHandlerMethods();

        /// <summary>
        /// Generates a PowerShell command to forcefully terminate processes by name, allowing for inclusion (name matches) and exclusion (name does not match prefixed with "!") criteria. 
        /// After Runs a powershell script and executes said command. It will kill all proceses which respect naming conditions
        /// <param name="limitScopeToCurrentUser">Will only terminate the procesess owned by current user. Default = true</param>
        /// <param name="args">strings which will determing correct processes to terminate</param>
        /// <para>
        /// Example of arguments: ("test", "!production") will generate a script that kills all processes that contain "test" but do not contain "production"
        /// </para>
        /// </summary> 
        public static string Kill_Process_ByName(bool limitScopeToCurrentUser = true, params string[] args)
        {
            return _processHandler.Kill_Process_ByName(limitScopeToCurrentUser, args);
        }
        /// <summary>
        /// Will search trough all opened procesess and kill a process with PID and TimeCreated saved in <paramref name="fullFileName"/> .txt file if processs has same 
        /// PID and TimeCreated
        /// </summary>
        public static string Kill_Process_ByPIDAndTimeCreated(string fullFileName)
        {
            return _processHandler.Kill_Process_ByPIDAndTimeCreated(fullFileName);
        }
        /// <summary>
        /// Saves the ID and start time of a specified process to a file.
        /// </summary>
        /// <param name="process">The process whose information is to be saved.</param>
        /// <param name="fullFileName">The full path and name of the file where the process information will be saved.</param>
        /// <remarks>
        /// If the provided filename is null or empty, the method will exit without performing any action.
        /// The process ID and start time are saved in the format "ID|Start Time".
        /// </remarks>
        public static string SaveProcessInfo(Process process, string fullFileName)
        {
            return _processHandler.SaveProcessInfo(process, fullFileName);
        }
        /// <summary>
        /// Terminates .NET processes whose full name or command line (full name of executing process) contains a specified filter string.
        /// </summary>
        /// <param name="filter">A string filter used to match against the process's full name or command line. 
        /// Only processes that contain this string in their full name or command line will be terminated.</param>
        public static string Kill_DotnetProcess_ByFullNameFilter(string filter)
        {
            return _processHandler.Kill_DotnetProcess_ByFullNameFilter(filter);
        }

        /// <summary>
        /// Terminates processes whose full name or command line (full name of executing process) contains a specified filter string and are owned by the current user.
        /// </summary>
        /// <param name="filter">Terminates process which full name contain filter parameter. For example, it can be a folder where .exe or process name</param>
        /// <param name="limitScopeToCurrentUser">Will only terminate the procesess owned by current user. Default = true</param>
        /// <remarks>
        /// This method uses Windows Management Instrumentation (WMI) to query active processes and matches the command line of each process against the provided filter. 
        /// It ensures that only processes owned by the current user are targeted for termination, minimizing the risk of affecting system or other user's processes.
        /// </remarks>
        public static string Kill_Process_ByFullNameFilter(string filter, bool limitScopeToCurrentUser = true)
        {
            return _processHandler.Kill_Process_ByFullNameFilter(filter, limitScopeToCurrentUser);
        }

        /// <summary>
        /// Terminates all processes that are listening on the specified TCP ports.
        /// </summary>
        /// <param name="ports">An array of integers representing the TCP ports. Processes listening on any of these ports will be terminated.</param>
        /// <remarks>
        /// This method uses the TCP connections from the local machine to identify and terminate processes based on the ports they are listening on. It 
        /// checks each connection to see if its local port is contained in the provided list of ports, and if so, attempts to terminate the associated process.
        /// </remarks>
        public static string Kill_Process_ByTcpPortListened(params int[] ports)
        {
            return _processHandler.Kill_Process_ByTcpPortListened(ports);
        }
    }
}
