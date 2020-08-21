using System;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Views;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Microcharts;
using Microcharts.Droid;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments.Home
{
    public sealed class HomeFragmentView : Fragment
    {
        private ChartView _chartViewSessions;
        private ChartView _chartViewVideos;
        private ChartView _chartViewReports;
        private ChartView _chartViewLanePosition;

        private readonly Context _activityContext;
        private readonly HomeFragmentViewPresenter _viewPresenter;

        //============================================================
        public HomeFragmentView(Context activityContext, User user)
        {
            _activityContext = activityContext;
            _viewPresenter = new HomeFragmentViewPresenter(activityContext, user);
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;
        }

        //============================================================
        private void ViewPresenterOnNotificationReceived(object sender, NotificationEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Utils.ShowToast(_activityContext, ex.Message, true);
                return;
            }

            switch (e.Command)
            {
                case NotificationCommand.HomeFragment_CreateSessionChart:
                {
                    _chartViewSessions.Chart = e.Data as Chart;
                    break;
                }
                case NotificationCommand.HomeFragment_CreateVideoChart:
                {
                    _chartViewVideos.Chart = e.Data as Chart;
                    break;
                }
                case NotificationCommand.HomeFragment_CreateReportChart:
                {
                    var charts = e.Data as Tuple<Chart, Chart>;
                    _chartViewReports.Chart = charts?.Item1;
                    _chartViewLanePosition.Chart = charts?.Item2;
                    break;
                }
            }
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
            await _viewPresenter.CreateSessionChart();
        }

        //============================================================
        private async Task CreateVideoChart()
        {
            await _viewPresenter.CreateVideoChart();
        }

        //============================================================
        private async Task CreateReportChart()
        {
            await _viewPresenter.CreateReportChart();
        }

        //============================================================
        private async void RefreshData()
        {
            var progressDialog = Utils.ShowProgressDialog(_activityContext, "Home", "Loading data ...");
            await CreateSessionChart();
            await CreateVideoChart();
            await CreateReportChart();
            progressDialog.Dismiss();
        }
    }
}