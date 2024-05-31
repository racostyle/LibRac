using System.Diagnostics;
using System.Threading.Tasks;

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
        /// <para>
        /// Example of arguments: ("test", "!production") will generate a script that kills all processes that contain "test" but do not contain "production"
        /// </para>
        /// </summary> 
        public static async Task KillProcess_ByName(params string[] args)
        {
            await _processHandler.KillProcess_ByName(args);
        }
        /// <summary>
        /// Will search trough all opened procesess and kill a process with PID and TimeCreated saved in <paramref name="fullFileName"/> .txt file if processs has same 
        /// PID and TimeCreated
        /// </summary>
        public static void KillProcess_ByPIDAndTimeCreated(string fullFileName)
        {
            _processHandler.KillProcess_ByPIDAndTimeCreated(fullFileName);
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
        public static void SaveProcessInfo(Process process, string fullFileName)
        {
            _processHandler.SaveProcessInfo(process, fullFileName);
        }
    }
}
