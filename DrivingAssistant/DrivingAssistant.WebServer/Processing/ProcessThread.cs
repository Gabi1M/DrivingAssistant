using System;
using System.Linq;
using System.Threading;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Models.Reports;
using DrivingAssistant.WebServer.Processing.Algorithms;
using DrivingAssistant.WebServer.Services.Generic;
using DrivingAssistant.WebServer.Tools;

namespace DrivingAssistant.WebServer.Processing
{
    public static class ProcessThread
    {
        //============================================================
        public static void ProcessSession(long sessionId, ProcessingAlgorithmType type)
        {
            new Thread(async () =>
            {
                using var sessionService = IDrivingSessionService.CreateNew();
                using var userService = IUserService.CreateNew();
                using var videoService = IVideoService.CreateNew();
                using var thumbnailService = IThumbnailService.CreateNew();
                using var reportService = IReportService.CreateNew();

                var session = await sessionService.GetById(sessionId);
                var user = await userService.GetById(session.UserId);
                session.Status = SessionStatus.Processing;
                await sessionService.SetAsync(session);
                var linkedVideos = await videoService.GetBySession(session.Id);
                switch (type)
                {
                    case ProcessingAlgorithmType.Lane_Departure_Warning:
                        {
                            var imageProcessor = new LaneDepartureWarningAlgorithm(LaneDepartureWarningAlgorithm.LaneDepartureWarningParameters.Default());
                            foreach (var video in linkedVideos.Where(x => !x.IsProcessed()))
                            {
                                var processedFilename = imageProcessor.ProcessVideo(video.Filepath, 10, out var result);
                                var processedVideo = new VideoRecording
                                {
                                    Filepath = processedFilename,
                                    Source = video.Source,
                                    Description = !string.IsNullOrEmpty(video.Description) ? video.Description + "_processed" : string.Empty,
                                    DateAdded = DateTime.Now,
                                    Id = -1,
                                    ProcessedId = -1,
                                    SessionId = video.SessionId,
                                };

                                var report = LaneDepartureWarningReport.FromVideoReport(result, video.Id);
                                processedVideo.Id = await videoService.SetAsync(processedVideo);
                                report.PdfPath = await Utils.CreateHtmlFromReport(report);
                                report.Id = await reportService.SetAsync(report);
                                video.ProcessedId = processedVideo.Id;
                                await videoService.SetAsync(video);

                                var thumbnail = new Thumbnail
                                {
                                    Id = -1,
                                    VideoId = processedVideo.Id,
                                    Filepath = Common.ExtractThumbnail(processedVideo.Filepath)
                                };
                                await thumbnailService.SetAsync(thumbnail);
                            }
                            break;
                        }
                }

                session.Status = SessionStatus.Processed;
                await sessionService.SetAsync(session);

                await Utils.SendEmail(user.Email, "Driving session: " + session.Name + " Finished Processing",
                    "Driving session: " + session.Name +
                    " Finished Processing. Additional information regarding the trip, as well as reports for individial videos, can be found in the app.");
            }).Start();
        }
    }
}
