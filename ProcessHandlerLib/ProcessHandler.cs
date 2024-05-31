using System.Diagnostics;
using System.Threading.Tasks;

namespace Librac.ProcessHandlerLib
{
    public static class ProcessHandler
    {
        private static readonly IProcessHandler _processHandler = new ProcessHandlerMethods();

        public static async Task KillProcess_ByName(params string[] args)
        {
            await _processHandler.KillProcess_ByName(args);
        }

        public static void KillProcess_ByPIDAndTimeCreated(string fullFileName)
        {
            _processHandler.KillProcess_ByPIDAndTimeCreated(fullFileName);
        }

        public static void SaveProcessInfo(Process process, string fullFileName)
        {
            _processHandler.SaveProcessInfo(process, fullFileName);
        }
    }
}
