using System;
using System.Linq;
using System.Threading.Tasks;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Models;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class HomeFragment : Fragment
    {
        private readonly User _user;
        private readonly SessionService _sessionService = new SessionService();
        private readonly VideoService _videoService = new VideoService();
        private readonly ReportService _reportService = new ReportService();

        private ChartView _chartViewSessions;
        private ChartView _chartViewVideos;
        private ChartView _chartViewReports;
        private ChartView _chartViewLanePosition;

        //============================================================
        public HomeFragment(User user)
        {
            _user = user;
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_home, container, false);
            SetupFragmentFields(view);
            RefreshData();

            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _chartViewSessions = view.FindViewById<ChartView>(Resource.Id.homeChartSessions);
            _chartViewVideos = view.FindViewById<ChartView>(Resource.Id.homeChartVideos);
            _chartViewReports = view.FindViewById<ChartView>(Resource.Id.homeChartFrames);
            _chartViewLanePosition = view.FindViewById<ChartView>(Resource.Id.homeChartLanePosition);
        }

        //============================================================
        private async Task CreateSessionChart()
        {
            try
            {
                var sessions = await _sessionService.GetByUserAsync(_user.Id);
                var sessionChartEntries = new[]
                {
                    new ChartEntry(sessions.Count())
                    {
                        Color = new SKColor(0, 255, 0),
                        Label = "Total",
                        ValueLabel = sessions.Count().ToString()
                    },
                    new ChartEntry(sessions.Count(x => x.Processed))
                    {
                        Color = new SKColor(0,0,255),
                        Label = "Processed",
                        ValueLabel = sessions.Count(x => x.Processed).ToString()
                    },
                    new ChartEntry(sessions.Count() - sessions.Count(x => x.Processed))
                    {
                        Color = new SKColor(255, 0,0),
                        Label = "Unprocessed",
                        ValueLabel = (sessions.Count() - sessions.Count(x => x.Processed)).ToString()
                    }
                };
                _chartViewSessions.Chart = new BarChart { Entries = sessionChartEntries, BackgroundColor = SKColor.Parse("#272929"), LabelTextSize = 25, Margin = 50 };
            }
            catch (Exception)
            {
                Toast.MakeText(Context, "Failed to load data for session chart!", ToastLength.Short).Show();
            }
        }

        //============================================================
        private async Task CreateVideoChart()
        {
            try
            {
                var videos = await _videoService.GetVideoByUserAsync(_user.Id);
                var videoChartEntries = new[]
                {
                    new ChartEntry(videos.Count())
                    {
                        Color = new SKColor(0, 255, 0),
                        Label = "Total",
                        ValueLabel = videos.Count().ToString()
                    },
                    new ChartEntry(videos.Count(x => x.IsProcessed()))
                    {
                        Color = new SKColor(0,0,255),
                        Label = "Processed",
                        ValueLabel = videos.Count(x => x.IsProcessed()).ToString()
                    },
                    new ChartEntry(videos.Count() - videos.Count(x => x.IsProcessed()))
                    {
                        Color = new SKColor(255, 0,0),
                        Label = "Unprocessed",
                        ValueLabel = (videos.Count() - videos.Count(x => x.IsProcessed())).ToString()
                    }
                };
                _chartViewVideos.Chart = new BarChart { Entries = videoChartEntries, BackgroundColor = SKColor.Parse("#272929"), LabelTextSize = 25, Margin = 50 };
            }
            catch (Exception)
            {
                Toast.MakeText(Context, "Failed to load data for video chart!", ToastLength.Short).Show();
            }
        }

        //============================================================
        private async Task CreateReportChart()
        {
            try
            {
                var reports = await _reportService.GetByUserAsync(_user.Id);
                var reportChartEntries = new[]
                {
                    new ChartEntry(reports.Sum(x => x.ProcessedFrames))
                    {
                        Color = new SKColor(0, 255, 0),
                        Label = "Total",
                        ValueLabel = reports.Sum(x => x.ProcessedFrames).ToString()
                    },
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


                _chartViewReports.Chart = new BarChart { Entries = reportChartEntries, BackgroundColor = SKColor.Parse("#272929"), LabelTextSize = 25, Margin = 50 };
                _chartViewLanePosition.Chart = new PieChart { Entries = positionChartEntries, BackgroundColor = SKColor.Parse("#272929"), LabelTextSize = 25, MaxValue = 100, LabelMode = LabelMode.RightOnly, Margin = 50 };
            }
            catch (Exception)
            {
                Toast.MakeText(Context, "Failed to load data for report charts!", ToastLength.Short).Show();
            }
        }

        //============================================================
        private async void RefreshData()
        {
            await CreateSessionChart();
            await CreateVideoChart();
            await CreateReportChart();
        }
    }
}