using System.Diagnostics;
using System.Threading.Tasks;

namespace Librac.ProcessHandlerLib
{
    internal interface IProcessHandler
    {
        Task KillProcess_ByName(params string[] args);
        void KillProcess_ByPIDAndTimeCreated(string fullFileName);
        void SaveProcessInfo(Process process, string fullFileName);
    }
}