using System;
using System.Globalization;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using DrivingAssistant.Core.Models.Reports;
using Microcharts;
using Microcharts.Droid;
using Newtonsoft.Json;
using SkiaSharp;

namespace DrivingAssistant.AndroidApp.Activities.Report
{
    [Activity(Label = "ReportActivityView", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ReportActivityView : Activity
    {
        private ChartView _roadPositionChart;
        private ChartView _framesChart;
        private TextView _textTotalFrames;
        private TextView _textSuccessFrames;
        private TextView _textFailFrames;
        private TextView _textSuccessRate;
        private TextView _textLeftSidePercent;
        private TextView _textRightSidePercent;
        private TextView _textLeftSideLineLength;
        private TextView _textRightSideLineLength;
        private TextView _textSpanLineAngle;
        private TextView _textSpanLineLength;
        private TextView _textLeftSideLineNumber;
        private TextView _textRightSideLineNumber;

        private Button _buttonDownload;

        private ReportActivityViewPresenter _viewPresenter;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_report);
            SetupActivityFields();

            _viewPresenter = new ReportActivityViewPresenter(this,
                JsonConvert.DeserializeObject<VideoRecording>(Intent?.GetStringExtra("video")!));
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;
            LoadData();
        }

        //============================================================
        private async void LoadData()
        {
            await _viewPresenter.Refresh();
        }

        //============================================================
        private void ViewPresenterOnNotificationReceived(object sender, NotificationEventArgs e)
        {
            switch (e.Command)
            {
                case NotificationCommand.ReportActivity_Refresh:
                {
                    var report = e.Data as LaneDepartureWarningReport;
                    PopulateFields(report);
                    break;
                }
                case NotificationCommand.ReportActivity_Download:
                {
                    Utils.ShowToast(this, "Report saved to: " + e.Data);
                    break;
                }
            }
        }

        //============================================================
        private void SetupActivityFields()
        {
            _roadPositionChart = FindViewById<ChartView>(Resource.Id.reportChartRoadPosition);
            _framesChart = FindViewById<ChartView>(Resource.Id.reportChartFrames);
            _textTotalFrames = FindViewById<TextView>(Resource.Id.reportTextTotalFrames);
            _textSuccessFrames = FindViewById<TextView>(Resource.Id.reportTextSuccessFrames);
            _textFailFrames = FindViewById<TextView>(Resource.Id.reportTextFailFrames);
            _textSuccessRate = FindViewById<TextView>(Resource.Id.reportTextSuccessRate);
            _textLeftSidePercent = FindViewById<TextView>(Resource.Id.reportTextLeftSidePercent);
            _textRightSidePercent = FindViewById<TextView>(Resource.Id.reportTextRightSidePercent);
            _textLeftSideLineLength = FindViewById<TextView>(Resource.Id.reportTextLeftSideLineLength);
            _textRightSideLineLength = FindViewById<TextView>(Resource.Id.reportTextRightSideLineLength);
            _textSpanLineAngle = FindViewById<TextView>(Resource.Id.reportTextSpanLineAngle);
            _textSpanLineLength = FindViewById<TextView>(Resource.Id.reportTextSpanLineLength);
            _textLeftSideLineNumber = FindViewById<TextView>(Resource.Id.reportTextLeftSideLineNumber);
            _textRightSideLineNumber = FindViewById<TextView>(Resource.Id.reportTextRightSideLineNumber);
            _buttonDownload = FindViewById<Button>(Resource.Id.reportButtonDownload);
            _buttonDownload.Click += ButtonDownloadOnClick;
        }

        //============================================================
        private void PopulateFields(LaneDepartureWarningReport report)
        {
            _textTotalFrames.Text = report.ProcessedFrames.ToString();
            _textSuccessFrames.Text = report.SuccessFrames.ToString();
            _textFailFrames.Text = report.FailFrames.ToString();
            _textSuccessRate.Text = report.SuccessRate.ToString(CultureInfo.InvariantCulture);
            _textLeftSidePercent.Text = report.LeftSidePercent.ToString(CultureInfo.InvariantCulture);
            _textRightSidePercent.Text = report.RightSidePercent.ToString(CultureInfo.InvariantCulture);
            _textLeftSideLineLength.Text = report.LeftSideLineLength.ToString(CultureInfo.InvariantCulture);
            _textRightSideLineLength.Text = report.RightSideLineLength.ToString(CultureInfo.InvariantCulture);
            _textSpanLineAngle.Text = report.SpanLineAngle.ToString(CultureInfo.InvariantCulture);
            _textSpanLineLength.Text = report.SpanLineLength.ToString(CultureInfo.InvariantCulture);
            _textLeftSideLineNumber.Text = report.LeftSideLineNumber.ToString();
            _textRightSideLineNumber.Text = report.RightSideLineNumber.ToString();

            var positionChartEntries = new[]
            {
                new ChartEntry(Convert.ToSingle(report.LeftSidePercent))
                {
                    Color = new SKColor(0, 255, 0),
                    Label = "Left Side",
                    ValueLabel = report.LeftSidePercent.ToString("##.##'%")
                },
                new ChartEntry(Convert.ToSingle(report.RightSidePercent))
                {
                    Color = new SKColor(255, 0,0),
                    Label = "Right Side",
                    ValueLabel = report.RightSidePercent.ToString("##.##'%")
                },
            };

            var framesChartEntries = new[]
            {
                new ChartEntry(report.SuccessFrames)
                {
                    Color = new SKColor(0,0,255),
                    Label = "Success",
                    ValueLabel = report.SuccessFrames.ToString()
                },
                new ChartEntry(report.FailFrames)
                {
                    Color = new SKColor(255, 0,0),
                    Label = "Fail",
                    ValueLabel = report.FailFrames.ToString()
                },
            };

            _roadPositionChart.Chart = new PieChart
            {
                Entries = positionChartEntries,
                BackgroundColor = SKColor.Parse("#272929"),
                LabelTextSize = 25,
                LabelMode = LabelMode.RightOnly,
                Margin = 50
            };

            _framesChart.Chart = new PieChart
            {
                Entries = framesChartEntries,
                BackgroundColor = SKColor.Parse("#272929"),
                LabelTextSize = 25,
                LabelMode = LabelMode.RightOnly,
                Margin = 50
            };
        }

        //============================================================
        private async void ButtonDownloadOnClick(object sender, EventArgs e)
        {
            await _viewPresenter.Download();
        }
    }
}