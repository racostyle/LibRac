using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace LibRac.PipeLines.Server
{
    internal class PipeServer_OneTime : IPipeServer
    {
        private readonly Action<string> _logger;
        private readonly PipeServerUtils _pipeServerUtils;
        private readonly Task _pipeServiceTask;

        private CancellationTokenSource _tokenSource;
        private NamedPipeServerStream _server;

        public bool IsActive => _server != null;
        private bool _isActive;

        public PipeServer_OneTime(Action<string> logger, PipeServerUtils pipeServerUtils, int timeoutInMils)
        {
            _logger = logger;
            _pipeServerUtils = pipeServerUtils;

            _server = pipeServerUtils.CreateNewServer();

            _tokenSource = new CancellationTokenSource();
            _pipeServiceTask = Task.Run(async () => await Listen_OneTime(timeoutInMils), _tokenSource.Token);

            _isActive = true;
        }

        private async Task Listen_OneTime(int timeoutInMils = 60000)
        {
            try
            {
                _logger?.Invoke("Waiting for client connection...");
                var connected = await _pipeServerUtils.WaitForConnectionAsyncWrapper(timeoutInMils, _server);
                if (!connected)
                {
                    _logger?.Invoke("Timeout waiting for client.");
                    return;
                }

                _logger?.Invoke("A client has connected.");

                using (var reader = new StreamReader(_server))
                {
                    await _pipeServerUtils.WaitForMessageAsync(reader, _tokenSource);
                }
            }
            catch (Exception ex)
            {
                _logger?.Invoke("Server error: " + ex.Message);
            }
            _logger?.Invoke("Pipeline Server Closed");
        }

        public void Dispose()
        {
            _isActive = false;
            _tokenSource?.Cancel();
            _server?.Close();
            _server?.Dispose();
            _logger?.Invoke("PipeServer Closed");
        }
    }
}
