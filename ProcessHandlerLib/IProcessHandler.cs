using System.Diagnostics;

namespace Librac.ProcessHandlerLib
{
    internal interface IProcessHandler
    {
        string Kill_Process_ByName(bool limitScopeToCurrentUser = true, params string[] args);
        string Kill_Process_ByPIDAndTimeCreated(string fullFileName);
        string SaveProcessInfo(Process process, string fullFileName);
        string Kill_DotnetProcess_ByFullNameFilter(string filter);
        string Kill_Process_ByFullNameFilter(string filter, bool limitScopeToCurrentUser = true);
        string Kill_Process_ByTcpPortListened(params int[] ports);
    }
}