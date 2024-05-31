using System;
using System.Diagnostics;

namespace Librac.DotnetLib
{
    internal class OutputWriter
    {
        Action<string> _writer;

        public OutputWriter(Action<string> writer)
        {
            _writer = writer;
        }

        internal void OutputDataRecieved(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _writer(e.Data);
            }
        }
        internal void ErrorDataRecieved(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _writer(e.Data);
            }
        }
    }
}
