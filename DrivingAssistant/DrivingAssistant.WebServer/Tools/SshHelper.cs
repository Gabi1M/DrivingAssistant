using System;
using Renci.SshNet;

namespace DrivingAssistant.WebServer.Tools
{
    public class SshHelper
    {
        private readonly SshClient _client;

        //======================================================//
        public SshHelper(string host, string username, string password)
        {
            _client = new SshClient(new ConnectionInfo(host, 22, username,
                new PasswordAuthenticationMethod(username, password)))
            {
                ConnectionInfo = {Timeout = TimeSpan.FromSeconds(5)}
            };
        }

        //======================================================//
        ~SshHelper()
        {
            _client.Dispose();
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
        public string SendCommand(string commandText)
        {
            var command = _client.CreateCommand(commandText);
            command.CommandTimeout = TimeSpan.FromSeconds(5);
            try
            {
               return command.Execute();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
