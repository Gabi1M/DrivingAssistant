using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Activities.Map;
using DrivingAssistant.AndroidApp.Activities.SessionEdit;
using DrivingAssistant.AndroidApp.Activities.VideoList;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments.Session
{
    public sealed class SessionFragmentView : Fragment
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

        private readonly Context _activityContext;
        private readonly SessionFragmentViewPresenter _viewPresenter;

        //============================================================
        public SessionFragmentView(Context activityContext, User user)
        {
            _activityContext = activityContext;
            _user = user;

            _viewPresenter = new SessionFragmentViewPresenter(activityContext, user);
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
                case NotificationCommand.SessionFragment_ItemClick:
                {
                    var view = e.Data as View;
                    view?.SetBackgroundResource(Resource.Drawable.list_element_border);
                    break;
                }
                case NotificationCommand.SessionFragment_Refresh:
                {
                    _listView.Adapter?.Dispose();
                    _listView.Adapter = new SessionViewModelAdapter(Activity, e.Data);
                    break;
                }
                case NotificationCommand.SessionFragment_Map:
                {
                    var session = e.Data as Core.Models.Session;
                    var intent = new Intent(_activityContext, typeof(MapActivityView));
                    intent.PutExtra("startPoint", JsonConvert.SerializeObject(session?.StartLocation));
                    intent.PutExtra("endPoint", JsonConvert.SerializeObject(session?.EndLocation));
                    intent.PutExtra("waypoints", JsonConvert.SerializeObject(session?.Waypoints));
                    StartActivity(intent);
                    break;
                }
                case NotificationCommand.SessionFragment_Original:
                {
                    var intent = new Intent(_activityContext, typeof(VideoListActivityView));
                    intent.PutExtra("videos", JsonConvert.SerializeObject(e.Data as IEnumerable<VideoRecording>));
                    StartActivity(intent);
                    break;
                }
                case NotificationCommand.SessionFragment_Processed:
                {
                    var intent = new Intent(_activityContext, typeof(VideoListActivityView));
                    intent.PutExtra("videos", JsonConvert.SerializeObject(e.Data as IEnumerable<VideoRecording>));
                    StartActivity(intent);
                    break;
                }
                case NotificationCommand.SessionFragment_Add:
                {
                    var intent = new Intent(_activityContext, typeof(SessionEditActivityView));
                    intent.PutExtra("user", JsonConvert.SerializeObject(_user));
                    StartActivityForResult(intent, 1234);
                    break;
                }
                case NotificationCommand.SessionFragment_Delete:
                {
                    if (Convert.ToBoolean(e.Data))
                    {
                        Utils.ShowToast(_activityContext, "Session successfully deleted!");
                    }
                    break;
                }
                case NotificationCommand.SessionFragment_Modify:
                {
                    var intent = new Intent(_activityContext, typeof(SessionEditActivityView));
                    intent.PutExtra("user", JsonConvert.SerializeObject(_user));
                    intent.PutExtra("session", JsonConvert.SerializeObject((e.Data as Core.Models.Session)));
                    StartActivityForResult(intent, 1234);
                    break;
                }
                case NotificationCommand.SessionFragment_Submit:
                {
                    if (Convert.ToBoolean(e.Data))
                    {
                        Utils.ShowToast(_activityContext, "Session successfully submitted!");
                    }
                    break;
                }
            }
        }

        //============================================================
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_sessions, container, false);
            SetupFragmentFields(view);
            SetupListAdapter();
            _viewPresenter.RefreshDataSource().ConfigureAwait(false);
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
        private void SetupListAdapter()
        {
            _listView.ChoiceMode = ChoiceMode.Single;
            _listView.ItemClick += OnItemClick;
        }

        //============================================================
        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _viewPresenter.ItemClick(e.Position, e.View);
        }

        //============================================================
        public override async void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            if (requestCode == 1234)
            {
                await _viewPresenter.RefreshDataSource();
            }
        }

        //============================================================
        private void OnMapButtonClick(object sender, EventArgs e)
        {
            _viewPresenter.MapButtonClick();
        }

        //============================================================
        private async void OnOriginalButtonClick(object sender, EventArgs e)
        {
            await _viewPresenter.OriginalButtonClick();
        }

        //============================================================
        private async void OnProcessedButtonClick(object sender, EventArgs e)
        {
            await _viewPresenter.ProcessedButtonClick();
        }

        //============================================================
        private void OnAddButtonClick(object sender, EventArgs e)
        {
            _viewPresenter.AddButtonClick();
        }

        //============================================================
        private void OnDeleteButtonClick(object sender, EventArgs e)
        {
            _viewPresenter.DeleteButtonClick();
        }

        //============================================================
        private void OnModifyButtonClick(object sender, EventArgs e)
        {
            _viewPresenter.ModifyButtonClick();
        }

        //============================================================
        private void OnSubmitButtonClick(object sender, EventArgs e)
        {
            _viewPresenter.SubmitButtonClick();
        }
    }
}