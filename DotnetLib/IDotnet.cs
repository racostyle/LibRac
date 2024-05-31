using Librac.ProcessHandlerLib;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Librac.DotnetLib
{
    internal interface IDotnet
    {
        Task Run_ExecuteAssemblyAsync(string assemblyPath, string args = "", Action? callback = null);
        Process Run_LaunchAssemblySimple(string assemblyPath, string args = "", Action? callback = null);
        string[] Run_LaunchAssembly(IProcessHandler? handler, string assemblyPath, bool hideWindow = false, bool runAsAdmin = false, string infoSaveLocation = "", Action<string>? outputCallback = null);
    }
}