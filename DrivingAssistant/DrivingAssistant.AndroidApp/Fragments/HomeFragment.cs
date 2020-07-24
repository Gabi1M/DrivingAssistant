using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
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
        private readonly SessionService _sessionService = new SessionService(Constants.ServerUri);
        private readonly MediaService _mediaService = new MediaService(Constants.ServerUri);

        private TextView _textTotalSessions;
        private TextView _textProcessedSessions;
        private TextView _textTotalMedia;
        private TextView _textProcessedMedia;
        private ChartView _chartViewSessions;
        private ChartView _chartViewMedia;

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
            _textTotalSessions = view.FindViewById<TextView>(Resource.Id.homeTextSessionNumber);
            _textProcessedSessions = view.FindViewById<TextView>(Resource.Id.homeTextProcessedSessionNumber);
            _textTotalMedia = view.FindViewById<TextView>(Resource.Id.homeTextMediaNumber);
            _textProcessedMedia = view.FindViewById<TextView>(Resource.Id.homeTextProcessedMediaNumber);
            _chartViewSessions = view.FindViewById<ChartView>(Resource.Id.chartViewSessions);
            _chartViewMedia = view.FindViewById<ChartView>(Resource.Id.chartViewMedia);
        }

        //============================================================
        private async void RefreshData()
        {
            var sessions = (await _sessionService.GetAsync()).Where(x => x.UserId == _user.Id).ToList();
            var medias = (await _mediaService.GetMediaAsync(_user.Id));

            _textTotalSessions.Text = sessions.Count.ToString();
            _textProcessedSessions.Text = sessions.Count(x => x.Processed).ToString();
            _textTotalMedia.Text = medias.Count(x => x.ProcessedId == -1).ToString();
            _textProcessedMedia.Text = medias.Count(x => x.IsProcessed()).ToString();

            var sessionChartEntries = new[]
            {
                new Entry(sessions.Count - sessions.Count(x => x.Processed))
                {
                    Color = new SKColor(255, 0,0)
                }, 
                new Entry(sessions.Count(x => x.Processed))
                {
                    Color = new SKColor(0,0,255)
                }, 
            };

            var mediaChartEntries = new[]
            {
                new Entry(medias.Count(x => x.ProcessedId == -1) - medias.Count(x => x.IsProcessed()))
                {
                    Color = new SKColor(255, 0,0)
                }, 
                new Entry(medias.Count(x => x.IsProcessed()))
                {
                    Color = new SKColor(0,0,255)
                },
            };

            _chartViewSessions.Chart = new DonutChart() {Entries = sessionChartEntries, BackgroundColor = SKColor.Parse("#272929") };
            _chartViewMedia.Chart = new DonutChart() {Entries = mediaChartEntries, BackgroundColor = SKColor.Parse("#272929") };
        }
    }
}