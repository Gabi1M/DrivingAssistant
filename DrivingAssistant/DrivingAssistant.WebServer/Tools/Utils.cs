using System;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace DrivingAssistant.WebServer.Tools
{
    public static class Utils
    {
        //============================================================
        public static string GetRandomFilename(string format, bool thumbnail = false)
        {
            string path;
            if (thumbnail)
            {
                var directory = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                if (!Directory.Exists(Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory)))
                {
                    Directory.CreateDirectory(Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory));
                }
                path = Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory, Path.GetRandomFileName()) + format;
                while (File.Exists(path))
                {
                    path = Path.Combine(Constants.ServerConstants.GetImageStoragePath(), directory, Path.GetRandomFileName()) + format;
                }
            }
            else
            {
                var directory = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                if (!Directory.Exists(Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory)))
                {
                    Directory.CreateDirectory(Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory));
                }
                path = Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory, Path.GetRandomFileName()) + format;
                while (File.Exists(path))
                {
                    path = Path.Combine(Constants.ServerConstants.GetVideoStoragePath(), directory, Path.GetRandomFileName()) + format;
                }
            }

            return path;
        }

        //============================================================
        public static async Task<string> SaveVideoStreamToFileAsync(Stream videoStream, string encoding)
        {
            var filepath = GetRandomFilename("." + encoding);
            await SaveStreamToFileAsync(videoStream, filepath);
            return filepath;
        }

        //============================================================
        private static async Task SaveStreamToFileAsync(Stream stream, string filepath)
        {
            await using var file = File.Create(filepath);
            await stream.CopyToAsync(file);
            file.Close();
        }

        //============================================================
        public static async Task SendEmail(string toAddress, string title, string message)
        {
            var mailMessage = new MimeMessage();
            mailMessage.To.Add(MailboxAddress.Parse(ParserOptions.Default, toAddress));
            mailMessage.From.Add(MailboxAddress.Parse(Constants.ServerConstants.SenderAddress));
            mailMessage.Subject = title;
            mailMessage.Body = new TextPart(TextFormat.Text)
            {
                Text = message
            };

            using var emailClient = new SmtpClient
            {
                ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true
            };
            emailClient.AuthenticationMechanisms.Remove("XOAUTH2");
            await emailClient.ConnectAsync(Constants.ServerConstants.MailHost, Constants.ServerConstants.MailPort);
            await emailClient.AuthenticateAsync(Constants.ServerConstants.SenderAddress, Constants.ServerConstants.SenderPassword);

            await emailClient.SendAsync(mailMessage);
            await emailClient.DisconnectAsync(true);
        }
    }
}
