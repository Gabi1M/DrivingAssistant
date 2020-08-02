using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Activities;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class SessionFragment : Fragment
    {
        private readonly User _user;

        private ListView _listView;
        private Button _mapButton;
        private Button _originalButton;
        private Button _processedButton;
        private Button _addButton;
        private Button _modifyButton;
        private Button _deleteButton;
        private Button _submitButton;

        private int _selectedPosition = -1;
        private View _selectedView;

        private readonly SessionService _sessionService = new SessionService();
        private ICollection<Session> _currentSessions;

        //============================================================
        public SessionFragment(User user)
        {
            _user = user;
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_sessions, container, false);
            SetupFragmentFields(view);
            SetupListAdapter();
            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _listView = view.FindViewById<ListView>(Resource.Id.sessionsListView);
            _mapButton = view.FindViewById<Button>(Resource.Id.sessionsButtonMap);
            _originalButton = view.FindViewById<Button>(Resource.Id.sessionsButtonViewOriginal);
            _processedButton = view.FindViewById<Button>(Resource.Id.sessionsButtonViewProcessed);
            _addButton = view.FindViewById<Button>(Resource.Id.sessionsButtonAdd);
            _modifyButton = view.FindViewById<Button>(Resource.Id.sessionsButtonModify);
            _deleteButton = view.FindViewById<Button>(Resource.Id.sessionsButtonDelete);
            _submitButton = view.FindViewById<Button>(Resource.Id.sessionsButtonSubmit);

            _mapButton.Click += OnMapButtonClick;
            _originalButton.Click += OnOriginalButtonClick;
            _processedButton.Click += OnProcessedButtonClick;
            _addButton.Click += OnAddButtonClick;
            _modifyButton.Click += OnModifyButtonClick;
            _deleteButton.Click += OnDeleteButtonClick;
            _submitButton.Click += OnSubmitButtonClick;
        }

        //============================================================
        private async void SetupListAdapter()
        {
            await RefreshDataSource();
            _listView.ChoiceMode = ChoiceMode.Single;
            _listView.ItemClick += OnItemClick;
        }

        //============================================================
        private async Task RefreshDataSource()
        {
            _currentSessions = (await _sessionService.GetByUserAsync(_user.Id)).ToList();
            _listView.Adapter?.Dispose();
            _listView.Adapter = new SessionViewModelAdapter(Activity, _currentSessions);
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedView?.SetBackgroundResource(0);
            e.View.SetBackgroundResource(Resource.Drawable.list_element_border);
            _selectedPosition = e.Position;
            _selectedView = e.View;
        }

        //============================================================
        public override async void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == 1234)
            {
                await RefreshDataSource();
            }
        }

        //============================================================
        private void OnMapButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No session selected!", ToastLength.Short).Show();
                return;
            }

            var session = _currentSessions.ElementAt(_selectedPosition);
            var intent = new Intent(Context, typeof(MapActivity));

            intent.PutExtra("startPoint", JsonConvert.SerializeObject(session.StartLocation));
            intent.PutExtra("endPoint", JsonConvert.SerializeObject(session.EndLocation));
            intent.PutExtra("waypoints", JsonConvert.SerializeObject(session.Waypoints));
            StartActivity(intent);
        }

        //============================================================
        private async void OnOriginalButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No session selected!", ToastLength.Short).Show();
                return;
            }

            var session = _currentSessions.ElementAt(_selectedPosition);
            var videoService = new VideoService();
            var originalVideos = session.Status == SessionStatus.Processed
                ? (await videoService.GetVideoBySessionAsync(session.Id)).Where(x => x.IsProcessed())
                : await videoService.GetVideoBySessionAsync(session.Id);
            var intent = new Intent(Context, typeof(VideoListActivity));
            intent.PutExtra("videos", JsonConvert.SerializeObject(originalVideos));
            StartActivity(intent);
        }

        //============================================================
        private async void OnProcessedButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No session selected!", ToastLength.Short).Show();
                return;
            }

            var session = _currentSessions.ElementAt(_selectedPosition);
            if (session.Status == SessionStatus.Unprocessed)
            {
                Toast.MakeText(Context, "Session already submited!", ToastLength.Short).Show();
                return;
            }

            var videoService = new VideoService();
            var processedVideos = (await videoService.GetVideoBySessionAsync(session.Id)).Where(x => !x.IsProcessed());
            var intent = new Intent(Context, typeof(VideoListActivity));
            intent.PutExtra("videos", JsonConvert.SerializeObject(processedVideos));
            StartActivity(intent);
        }

        //============================================================
        private void OnAddButtonClick(object sender, EventArgs e)
        {
            var intent = new Intent(Context, typeof(SessionEditActivity));
            intent.PutExtra("user", JsonConvert.SerializeObject(_user));
            StartActivityForResult(intent, 1234);
        }

        //============================================================
        private void OnModifyButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No session selected!", ToastLength.Short).Show();
                return;
            }

            var session = _currentSessions.ElementAt(_selectedPosition);

            if (session.Status != SessionStatus.Unprocessed)
            {
                Toast.MakeText(Context, "Session already submited!", ToastLength.Short).Show();
                return;
            }

            var intent = new Intent(Context, typeof(SessionEditActivity));
            intent.PutExtra("user", JsonConvert.SerializeObject(_user));
            intent.PutExtra("session", JsonConvert.SerializeObject(session));
            StartActivityForResult(intent, 1234);
        }

        //============================================================
        private async void OnSubmitButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No session selected!", ToastLength.Short).Show();
                return;
            }

            var session = _currentSessions.ElementAt(_selectedPosition);
            if (session.Status != SessionStatus.Unprocessed)
            {
                Toast.MakeText(Context, "Session already submited!", ToastLength.Short).Show();
                return;
            }

            Toast.MakeText(Context, "Session submitted! It will be available shortly!", ToastLength.Long).Show();
            await _sessionService.SubmitAsync(session.Id);
            await RefreshDataSource();
        }

        //============================================================
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No session selected!", ToastLength.Short).Show();
                return;
            }

            var alert = new AlertDialog.Builder(Context);
            alert.SetTitle("Confirm Delete");
            alert.SetMessage("Action cannot be undone");
            alert.SetPositiveButton("Delete", async (o, args) =>
            {
                var session = _currentSessions.ElementAt(_selectedPosition);
                await _sessionService.DeleteAsync(session.Id);
                Toast.MakeText(Context, "Session deleted!", ToastLength.Short).Show();
                await RefreshDataSource();
            });

            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }
    }
}