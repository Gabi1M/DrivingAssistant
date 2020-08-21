using System;
using Renci.SshNet;

namespace DrivingAssistant.WebServer.Tools
{
    public class SshHelper : IDisposable
    {
        private readonly SshClient _client;

        //======================================================//
        public SshHelper(string host, string username, string password, int timeoutSeconds = 5)
        {
            _client = new SshClient(new ConnectionInfo(host, 22, username,
                new PasswordAuthenticationMethod(username, password)))
            {
                ConnectionInfo =
                {
                    Timeout = TimeSpan.FromSeconds(timeoutSeconds)
                }
            };
        }

        //======================================================//
        public void Connect()
        {
            _client.Connect();
        }

        //======================================================//
        public void Disconnect()
        {
            _client.Disconnect();
        }

        //======================================================//
        public string SendCommand(string commandText, int timeoutSeconds = 5)
        {
            var command = _client.CreateCommand(commandText);
            command.CommandTimeout = TimeSpan.FromSeconds(timeoutSeconds);
            try
            {
               return command.Execute();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //======================================================//
        public void Dispose()
        {
            if (_client.IsConnected)
            {
                _client.Disconnect();
            }
            _client.Dispose();
        }
    }
}
