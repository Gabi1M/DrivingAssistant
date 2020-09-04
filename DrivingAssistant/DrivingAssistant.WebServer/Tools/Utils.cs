using System;
using System.IO;
using System.Threading.Tasks;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Models.Reports;
using DrivingAssistant.WebServer.Services.Generic;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace DrivingAssistant.WebServer.Tools
{
    public static class Utils
    {
        //============================================================
        public static string GetRandomFilename(string format, FileType fileType)
        {
            var path = string.Empty;
            switch (fileType)
            {
                case FileType.Video:
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
                    break;
                }
                case FileType.Thumbnail:
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
                    break;
                }
                case FileType.Report:
                {
                    var directory = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString();
                    if (!Directory.Exists(Path.Combine(Constants.ServerConstants.GetReportStoragePath(), directory)))
                    {
                        Directory.CreateDirectory(Path.Combine(Constants.ServerConstants.GetReportStoragePath(), directory));
                    }
                    path = Path.Combine(Constants.ServerConstants.GetReportStoragePath(), directory, Path.GetRandomFileName()) + format;
                    while (File.Exists(path))
                    {
                        path = Path.Combine(Constants.ServerConstants.GetReportStoragePath(), directory, Path.GetRandomFileName()) + format;
                    }
                    break;
                }
            }

            return path;
        }

        //============================================================
        public static async Task<string> SaveVideoStreamToFileAsync(Stream videoStream, string encoding)
        {
            var filepath = GetRandomFilename("." + encoding, FileType.Video);
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

        //============================================================
        public static async Task<string> CreatePdfFromReport(LaneDepartureWarningReport report)
        {
            using var videoService = IVideoService.CreateNew();
            using var sessionService = IDrivingSessionService.CreateNew();
            var video = await videoService.GetById(report.VideoId);
            var drivingSession = await sessionService.GetById(video.SessionId);

            var htmlText = "<h1 style=\"text-align: center; font-weight: bold;\">Report for video \"" + video.Description + "\"</h1>";
            htmlText += "<p>Video ID: \"" + video.Id + "\"</p>";
            htmlText += "<p>Video name: \"" + video.Description + "\"</p>";
            htmlText += "<p>Session name: \"" + drivingSession.Name + "\"</p>";
            htmlText += "<p>Session time period: \"" + drivingSession.StartDateTime + " " + drivingSession.EndDateTime + "\"</p>";
            htmlText += "<p>Session start location: \"" + drivingSession.StartLocation.PointToString() + "\"</p>";
            htmlText += "<p>Session end location: \"" + drivingSession.EndLocation.PointToString() + "\"</p>";
            htmlText += "<p>Number of waypoints: \"" + drivingSession.Waypoints.Count + "\"</p>";
            htmlText += "<p>Total processed frames: \"" + report.ProcessedFrames + "\"</p>";
            htmlText += "<p>Successfully processed frames: \"" + report.SuccessFrames + "\"</p>";
            htmlText += "<p>Unsuccessfully processed frames: \"" + report.FailFrames + "\"</p>";
            htmlText += "<p>Success rate: \"" + report.SuccessRate + "\"</p>";
            htmlText += "<p>Left side percent: \"" + report.LeftSidePercent + "\"</p>";
            htmlText += "<p>Right side percent: \"" + report.RightSidePercent + "\"</p>";
            htmlText += "<p>Average left side line length: \"" + report.LeftSideLineLength + "\"</p>";
            htmlText += "<p>Average right side line length: \"" + report.RightSideLineLength + "\"</p>";
            htmlText += "<p>Average span line length: \"" + report.SpanLineLength + "\"</p>";
            htmlText += "<p>Average span line angle: \"" + report.SpanLineAngle + "\"</p>";
            htmlText += "<p>Average left side line number: \"" + report.LeftSideLineNumber + "\"</p>";
            htmlText += "<p>Average right side line number: \"" + report.RightSideLineNumber + "\"</p>";

            var filename = GetRandomFilename(".html", FileType.Report);
            await File.WriteAllTextAsync(filename, htmlText);
            
            return filename;
        }
    }
}
