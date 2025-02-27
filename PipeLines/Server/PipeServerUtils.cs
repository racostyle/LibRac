using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;

namespace LibRac.PipeLines.Server
{
    public class PipeServerUtils
    {
        private readonly Action<string> _logger;
        private readonly string _pipeName;

        public PipeServerUtils(Action<string> logger, string pipeName)
        {
            _logger = logger;
            _pipeName = pipeName;
        }

        internal NamedPipeServerStream CreateNewServer()
        {
            try
            {
                var server = new NamedPipeServerStream(
                    _pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Message,
                    PipeOptions.Asynchronous);

                _logger?.Invoke($"Pipeline server '{_pipeName}' initialized");
                return server;
            }
            catch (Exception ex)
            {
                _logger?.Invoke($"ERROR: {ex}");
                throw;
            }
        }

        internal async Task WaitForMessageAsync(StreamReader reader, CancellationTokenSource tokenSource)
        {
            while (!tokenSource.IsCancellationRequested)
            {
                string message = await reader.ReadLineAsync();

                if (tokenSource.IsCancellationRequested)
                    return;

                if (message == null)
                {
                    _logger?.Invoke("Client has disconnected.");
                    return;  // Exit the reading loop and listen for new connection
                }

                _logger?.Invoke("Received from client: " + message);
            }
        }

        internal async Task<NamedPipeServerStream> ResetConnection(NamedPipeServerStream server)
        {
            if (server.IsConnected)
                server.Disconnect();

            server.Close();
            server.Dispose();

            await Task.Delay(100);

            return CreateNewServer(); // Recreate the server for the next client
        }

        internal async Task<bool> WaitForConnectionAsyncWrapper(int timeoutInMils, NamedPipeServerStream server)
        {
            using (CancellationTokenSource cts = new CancellationTokenSource(timeoutInMils))
            {
                try
                {
                    await server.WaitForConnectionAsync(cts.Token);
                    return server.IsConnected;
                }
                catch (OperationCanceledException)
                {
                    return false;
                }
            }
        }
    }
}
