using System;
using System.Diagnostics;

namespace LibRac.DotNet
{
    internal class OutputWriter
    {
        Action<string> _writer;

        public OutputWriter(Action<string> writer)
        {
            _writer = writer;
        }

        internal void OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _writer(e.Data);
            }
        }
        internal void ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                _writer(e.Data);
            }
        }
    }
}
