using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Microcharts;
using SkiaSharp;

namespace DrivingAssistant.AndroidApp.Fragments.Home
{
    public class HomeFragmentViewPresenter : ViewPresenter
    {
        private readonly User _user;
        private readonly SessionService _sessionService = new SessionService();
        private readonly VideoService _videoService = new VideoService();
        private readonly ReportService _reportService = new ReportService();

        //============================================================
        public HomeFragmentViewPresenter(Context context, User user)
        {
            _context = context;
            _user = user;
        }

        //============================================================
        public async Task CreateSessionChart()
        {
            try
            {
                var sessions = await _sessionService.GetByUserAsync(_user.Id);
                var sessionChartEntries = new[]
                {
                    /*new ChartEntry(sessions.Count())
                    {
                        Color = new SKColor(0, 255, 0),
                        Label = "Total",
                        ValueLabel = sessions.Count().ToString()
                    },*/
                    new ChartEntry(sessions.Count(x => x.Status == SessionStatus.Processed))
                    {
                        Color = new SKColor(0,0,255),
                        Label = "Processed",
                        ValueLabel = sessions.Count(x => x.Status == SessionStatus.Processed).ToString()
                    },
                    new ChartEntry(sessions.Count() - sessions.Count(x => x.Status == SessionStatus.Processed))
                    {
                        Color = new SKColor(255, 0,0),
                        Label = "Unprocessed",
                        ValueLabel = (sessions.Count() - sessions.Count(x => x.Status == SessionStatus.Processed)).ToString()
                    }
                };
                var chart = new PieChart
                {
                    Entries = sessionChartEntries, BackgroundColor = SKColor.Parse("#272929"), LabelTextSize = 25,
                    LabelMode = LabelMode.RightOnly, Margin = 50
                };
                Notify(new NotificationEventArgs(NotificationCommand.HomeFragment_CreateSessionChart, chart));
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.HomeFragment_CreateSessionChart, ex));
            }
        }

        //============================================================

        public async Task CreateVideoChart()
        {
            try
            {
                var videos = await _videoService.GetVideoByUserAsync(_user.Id);
                var videoChartEntries = new[]
                {
                    /*new ChartEntry(videos.Count())
                    {
                        Color = new SKColor(0, 255, 0),
                        Label = "Total",
                        ValueLabel = videos.Count().ToString()
                    },*/
                    new ChartEntry(videos.Count(x => x.IsProcessed()))
                    {
                        Color = new SKColor(0,0,255),
                        Label = "Processed",
                        ValueLabel = videos.Count(x => x.IsProcessed()).ToString()
                    },
                    new ChartEntry(videos.Count(x => !x.IsProcessed()))
                    {
                        Color = new SKColor(255, 0,0),
                        Label = "Unprocessed",
                        ValueLabel = (videos.Count() - videos.Count(x => x.IsProcessed())).ToString()
                    }
                };
                var chart = new PieChart { Entries = videoChartEntries, BackgroundColor = SKColor.Parse("#272929"), LabelTextSize = 25, LabelMode = LabelMode.RightOnly, Margin = 50 };
                Notify(new NotificationEventArgs(NotificationCommand.HomeFragment_CreateVideoChart, chart));
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.HomeFragment_CreateVideoChart, ex));
            }
        }

        //============================================================
        public async Task CreateReportChart()
        {
            try
            {
                var reports = await _reportService.GetByUserAsync(_user.Id);
                var reportChartEntries = new[]
                {
                    /*new ChartEntry(reports.Sum(x => x.ProcessedFrames))
                    {
                        Color = new SKColor(0, 255, 0),
                        Label = "Total",
                        ValueLabel = reports.Sum(x => x.ProcessedFrames).ToString()
                    },*/
                    new ChartEntry(reports.Sum(x => x.SuccessFrames))
                    {
                        Color = new SKColor(0,0,255),
                        Label = "Success",
                        ValueLabel = reports.Sum(x => x.SuccessFrames).ToString()
                    },
                    new ChartEntry(reports.Sum(x => x.FailFrames))
                    {
                        Color = new SKColor(255, 0,0),
                        Label = "Fail",
                        ValueLabel = reports.Sum(x => x.FailFrames).ToString()
                    },
                };

                var positionChartEntries = new[]
                {
                    new ChartEntry(Convert.ToSingle(reports.Average(x => x.LeftSidePercent)))
                    {
                        Color = new SKColor(0, 255, 0),
                        Label = "Left Side",
                        ValueLabel = reports.Average(x => x.LeftSidePercent).ToString("##.##'%")
                    },
                    new ChartEntry(Convert.ToSingle(reports.Average(x => x.RightSidePercent)))
                    {
                        Color = new SKColor(255, 0,0),
                        Label = "Right Side",
                        ValueLabel = reports.Average(x => x.RightSidePercent).ToString("##.##'%")
                    },
                };


                var chartReport = new PieChart
                {
                    Entries = reportChartEntries, BackgroundColor = SKColor.Parse("#272929"), LabelTextSize = 25,
                    LabelMode = LabelMode.RightOnly, Margin = 50
                };
                var chartLanePosition = new PieChart
                {
                    Entries = positionChartEntries, BackgroundColor = SKColor.Parse("#272929"), LabelTextSize = 25,
                    MaxValue = 100, LabelMode = LabelMode.RightOnly, Margin = 50
                };

                Notify(new NotificationEventArgs(NotificationCommand.HomeFragment_CreateReportChart, new Tuple<Chart, Chart>(chartReport, chartLanePosition)));
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.HomeFragment_CreateReportChart, ex));
            }
        }
    }
}