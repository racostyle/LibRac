using System;

namespace LibRac.Shell.Logger
{
    internal class ActionLogger : IShellLogger
    {
        private readonly Action<string> _logger;

        public ActionLogger(Action<string> logger)
        {
            _logger = logger;
        }

        public void Log(string message)
        {
            _logger.Invoke(message);
        }
    }
}
