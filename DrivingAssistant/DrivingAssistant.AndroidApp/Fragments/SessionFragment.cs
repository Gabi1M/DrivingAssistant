using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Models;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class SessionFragment : Fragment
    {
        private ListView _listView;
        private Button _addButton;
        private Button _modifyButton;
        private Button _deleteButton;

        private int _selectedPosition = -1;
        private View _selectedView;

        private SessionService _sessionService;
        private ICollection<Session> _currentSessions;

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_sessions, container, false);
            _sessionService = new SessionService("http://192.168.100.234:3287");
            SetupFragmentFields(view);
            SetupListAdapter();
            return view;
        }

        //============================================================
        private void SetupFragmentFields(View view)
        {
            _listView = view.FindViewById<ListView>(Resource.Id.sessionsListView);
            _addButton = view.FindViewById<Button>(Resource.Id.sessionsButtonAdd);
            _modifyButton = view.FindViewById<Button>(Resource.Id.sessionsButtonModify);
            _deleteButton = view.FindViewById<Button>(Resource.Id.sessionsButtonDelete);

            _addButton.Click += OnAddButtonClick;
            _modifyButton.Click += OnModifyButtonClick;
            _deleteButton.Click += OnDeleteButtonClick;
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
            _currentSessions = await _sessionService.GetAsync();
            _listView.Adapter?.Dispose();
            _listView.Adapter = new SessionViewModelAdapter(Activity, _currentSessions);
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedView?.SetBackgroundColor(Color.White);
            e.View.SetBackgroundColor(Color.Aqua);
            _selectedPosition = e.Position;
            _selectedView = e.View;
        }

        //============================================================
        private void OnAddButtonClick(object sender, EventArgs e)
        {
            //TODO
        }

        //============================================================
        private void OnModifyButtonClick(object sender, EventArgs e)
        {
            //TODO
        }

        //============================================================
        private async void OnDeleteButtonClick(object sender, EventArgs e)
        {
            if (_selectedPosition == -1)
            {
                Toast.MakeText(Context, "No session selected!", ToastLength.Short).Show();
                return;
            }

            var session = _currentSessions.ElementAt(_selectedPosition);
            await _sessionService.DeleteAsync(session.Id);
            Toast.MakeText(Context, "Session deleted!", ToastLength.Short).Show();
            await RefreshDataSource();
        }
    }
}