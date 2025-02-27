using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace LibRac.Pipelines
{
    /// <summary>
    /// Manages client-side operations for named pipe communication.
    /// </summary>
    public class PipeClient : IDisposable
    {
        private readonly NamedPipeClientStream _client;
        private readonly Action<string> _logger;
        private readonly StreamWriter _writer;

        /// <summary>
        /// Initializes a new instance of the PipeClient class, connects to the server,
        /// and prepares for sending messages.
        /// </summary>
        /// <param name="pipeName">The name of the pipe to connect to.</param>
        /// <param name="logger">An action delegate to log messages.</param>
        /// <param name="timeoutInMils">The timeout in milliseconds for connecting to the server.</param>
        /// <remarks>
        /// This constructor attempts to connect to the named pipe server and initializes a StreamWriter
        /// for sending messages if the connection is successful. 
        /// <para><c>!!! The class should be used within an 'using' statement to ensure proper disposal of resources 
        /// or call Dispose() manually !!!</c>></para>
        /// </remarks>
        public PipeClient(string pipeName, Action<string> logger, int timeoutInMils = 5000)
        {
            _logger = logger;
            _client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut);

            CreateClientAndConnectToServer(_client, _logger, timeoutInMils);

            if (_client.IsConnected)
                _writer = new StreamWriter(_client);
        }

        private void CreateClientAndConnectToServer(NamedPipeClientStream client, Action<string> logger, int timeoutInMils)
        {
            try
            {
                logger?.Invoke("Client connecting...");
                client.Connect(timeoutInMils);
                logger?.Invoke("Client connected.");
            }
            catch (Exception ex)
            {
                logger?.Invoke("Error: " + ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Sends a message to the connected pipe server.
        /// </summary>
        /// <param name="message">The message to send to the server.</param>
        /// <remarks>
        /// This method writes a message to the server using a StreamWriter. If the client is not connected,
        /// a log message is generated. It handles any exceptions that might occur during the message sending process.
        /// </remarks>
        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (_writer == null)
                {
                    _logger?.Invoke("Client not connected!");
                    return;
                }
                _writer.AutoFlush = true;
                _logger?.Invoke("Sending message via Pipelines...");
                await _writer.WriteLineAsync(message);
            }
            catch (Exception ex)
            {
                _logger?.Invoke("Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Releases all resources used by the PipeClient.
        /// </summary>
        /// <remarks>
        /// This method disposes of the StreamWriter and the NamedPipeClientStream, ensuring no resource leaks.
        /// Always use this class within a 'using' statement to guarantee that resources are properly cleaned up.
        /// </remarks>
        public void Dispose()
        {
            _writer?.Dispose();
            _client?.Dispose();
        }
    }
}
