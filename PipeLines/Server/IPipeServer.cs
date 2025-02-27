using System;

namespace LibRac.PipeLines.Server
{
    public interface IPipeServer : IDisposable
    {
        public bool IsActive { get; }
    }
}
