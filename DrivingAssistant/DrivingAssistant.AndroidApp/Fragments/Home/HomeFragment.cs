using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Microcharts;
using Microcharts.Droid;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments.Home
{
    public sealed class HomeFragment : Fragment
    {
        private ChartView _chartViewSessions;
        private ChartView _chartViewVideos;
        private ChartView _chartViewReports;
        private ChartView _chartViewLanePosition;

        private readonly HomeFragmentPresenter _presenter;

        //============================================================
        public HomeFragment(User user)
        {
            _presenter = new HomeFragmentPresenter(Context, user);
            _presenter.OnPropertyChanged += PresenterOnPropertyChanged;
        }

        //============================================================
        private void PresenterOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Toast.MakeText(Context, ex.Message, ToastLength.Long)?.Show();
                return;
            }

            switch (e.Command)
            {
                case NotifyCommand.HomeFragment_CreateSessionChart:
                {
                    _chartViewSessions.Chart = e.Data as Chart;
                    break;
                }
                case NotifyCommand.HomeFragment_CreateVideoChart:
                {
                    _chartViewVideos.Chart = e.Data as Chart;
                    break;
                }
                case NotifyCommand.HomeFragment_CreateReportChart:
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
            await _presenter.CreateSessionChart();
        }

        //============================================================
        private async Task CreateVideoChart()
        {
            await _presenter.CreateVideoChart();
        }

        //============================================================
        private async Task CreateReportChart()
        {
            await _presenter.CreateReportChart();
        }

        //============================================================
        private async void RefreshData()
        {
            var progressDialog = new ProgressDialog(Context);
            progressDialog.SetMessage("Loading Data...");
            progressDialog.Show();
            await CreateSessionChart();
            await CreateVideoChart();
            await CreateReportChart();
            progressDialog.Dismiss();
        }
    }
}