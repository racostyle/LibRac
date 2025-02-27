using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace LibRac.PipeLines.Server
{
    internal class PipeServer_LimitedConnections : IPipeServer
    {
        private readonly Action<string> _logger;
        private readonly PipeServerUtils _pipeServerUtils;
        private readonly Task _pipeServiceTask;

        private CancellationTokenSource _tokenSource;
        private NamedPipeServerStream _server;

        public bool IsActive => _isActive;
        private bool _isActive;

        public PipeServer_LimitedConnections(Action<string> logger, PipeServerUtils pipeServerUtils, int timeoutInMils, uint connectionLimit)
        {
            _logger = logger;
            _pipeServerUtils = pipeServerUtils;

            _server = pipeServerUtils.CreateNewServer();

            _tokenSource = new CancellationTokenSource();
            _pipeServiceTask = Task.Run(async () => await Listen(connectionLimit, timeoutInMils), _tokenSource.Token);

            _isActive = true;
        }

        private async Task Listen(uint connectionLimit, int timeoutInMils = Timeout.Infinite)
        {
            while (!_tokenSource.IsCancellationRequested && connectionLimit > 0)
            {
                try
                {
                    _logger?.Invoke("Waiting for client connection...");
                    var connected = await _pipeServerUtils.WaitForConnectionAsyncWrapper(timeoutInMils, _server);
                    if (!connected)
                    {
                        _logger?.Invoke("Timeout waiting for client.");
                        continue;
                    }
                    else
                        connectionLimit--;

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
                finally
                {
                    if (!_tokenSource.IsCancellationRequested && connectionLimit > 0)
                        _server = await _pipeServerUtils.ResetConnection(_server);
                }
            }
            Dispose();
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
