using System.Diagnostics;

namespace Librac.ProcessHandlerLib
{
    internal interface IProcessHandler
    {
        void Kill_Process_ByName(params string[] args);
        void Kill_Process_ByPIDAndTimeCreated(string fullFileName);
        void SaveProcessInfo(Process process, string fullFileName);
        void Kill_DotnetProcess_ByFullNameFilter(string filter);
        void Kill_CurrentUserProcess_ByFullNameFilter(string filter);
        void Kill_Process_ByTcpPortListened(params int[] ports);
    }
}