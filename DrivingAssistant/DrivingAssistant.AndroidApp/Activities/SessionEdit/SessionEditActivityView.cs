using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Activities.Video;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace DrivingAssistant.AndroidApp.Activities.SessionEdit
{
    [Activity(Label = "SessionEditActivity")]
    public class SessionEditActivityView : Activity
    {
        private TextInputEditText _textDescription;
        private TextView _labelStartDateTime;
        private TextView _labelEndDateTime;
        private TextView _labelStartLocation;
        private TextView _labelEndLocation;
        private TextView _labelWaypoints;
        private ListView _videoListView;
        private Button _videoButtonAdd;
        private Button _videoButtonModify;
        private Button _videoButtonDelete;
        private Button _videoButtonView;
        private Button _buttonSubmit;

        private User _user;
        private SessionEditActivityViewPresenter _viewPresenter;

        //============================================================
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_session_edit);
            SetupActivityFields();

            _user = JsonConvert.DeserializeObject<User>(Intent?.GetStringExtra("user")!);

            DrivingSession currentDrivingSession = null;
            if (Intent.HasExtra("session"))
            {
                currentDrivingSession = JsonConvert.DeserializeObject<DrivingSession>(Intent.GetStringExtra("session")!);
                _textDescription.Text = currentDrivingSession.Name;
                _labelStartDateTime.Text = currentDrivingSession.StartDateTime.ToString(Constants.DateTimeFormat);
                _labelEndDateTime.Text = currentDrivingSession.EndDateTime.ToString(Constants.DateTimeFormat);
                _labelStartLocation.Text = currentDrivingSession.StartLocation.X + " " + currentDrivingSession.StartLocation.Y;
                _labelEndLocation.Text = currentDrivingSession.EndLocation.X + " " + currentDrivingSession.EndLocation.Y;
                _labelWaypoints.Text = currentDrivingSession.Waypoints.Count + " Selected";
                _videoListView.Adapter = new VideoThumbnailViewModelAdapter(this, (await (new VideoService()).GetVideoBySessionAsync(currentDrivingSession.Id)).ToList());
            }

            var location = await Geolocation.GetLocationAsync();
            _viewPresenter = new SessionEditActivityViewPresenter(this, currentDrivingSession, _user, location);
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;
        }

        //============================================================
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //============================================================
        private void ViewPresenterOnNotificationReceived(object sender, NotificationEventArgs e)
        {
            if (e.Data is Exception ex)
            {
                Utils.ShowToast(this, ex.Message, true);
                return;
            }

            switch (e.Command)
            {
                case NotificationCommand.SessionEditActivity_Back:
                    {
                        base.OnBackPressed();
                        break;
                    }
                case NotificationCommand.SessionEditActivity_ItemClick:
                    {
                        var view = e.Data as View;
                        view?.SetBackgroundResource(Resource.Drawable.list_element_border);
                        break;
                    }
                case NotificationCommand.SessionEditActivity_VideoRefresh:
                    {
                        _videoListView.Adapter?.Dispose();
                        _videoListView.Adapter = new VideoThumbnailViewModelAdapter(this, e.Data as List<VideoRecording>);
                        break;
                    }
                case NotificationCommand.SessionEditActivity_VideoView:
                    {
                        var intent = new Intent(this, typeof(VideoActivityView));
                        intent.PutExtra("video", JsonConvert.SerializeObject(e.Data as VideoRecording));
                        StartActivity(intent);
                        break;
                    }
                case NotificationCommand.SessionEditActivity_StartDate:
                    {
                        _labelStartDateTime.Text = (e.Data as DateTime?)?.ToString(Constants.DateTimeFormat);
                        break;
                    }
                case NotificationCommand.SessionEditActivity_EndDate:
                    {
                        _labelEndDateTime.Text = (e.Data as DateTime?)?.ToString(Constants.DateTimeFormat);
                        break;
                    }
                case NotificationCommand.SessionEditActivity_StartLocation:
                    {
                        var startLocation = e.Data as LocationPoint;
                        _labelStartLocation.Text = startLocation.X + " " + startLocation.Y;
                        break;
                    }
                case NotificationCommand.SessionEditActivity_EndLocation:
                    {
                        var endLocation = e.Data as LocationPoint;
                        _labelEndLocation.Text = endLocation.X + " " + endLocation.Y;
                        break;
                    }
                case NotificationCommand.SessionEditActivity_Waypoints:
                    {
                        var waypoints = e.Data as List<LocationPoint>;
                        _labelWaypoints.Text = waypoints.Count + " Selected";
                        break;
                    }
                case NotificationCommand.SessionEditActivity_Submit:
                    {
                        Finish();
                        break;
                    }
            }
        }

        //============================================================
        private void SetupActivityFields()
        {
            _textDescription = FindViewById<TextInputEditText>(Resource.Id.sessionEditTextDescription);
            _labelStartDateTime = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedStartDateTime);
            _labelEndDateTime = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedEndDateTime);
            _labelStartLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedStartPosition);
            _labelEndLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedEndPosition);
            _labelWaypoints = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedWaypoints);
            _videoListView = FindViewById<ListView>(Resource.Id.sessionEditVideoList);
            _videoButtonAdd = FindViewById<Button>(Resource.Id.videosButtonAdd);
            _videoButtonModify = FindViewById<Button>(Resource.Id.videosButtonModify);
            _videoButtonDelete = FindViewById<Button>(Resource.Id.videosButtonDelete);
            _videoButtonView = FindViewById<Button>(Resource.Id.videosButtonView);
            _buttonSubmit = FindViewById<Button>(Resource.Id.sessionEditButtonSubmit);

            _labelStartDateTime.Click += OnStartDateClick;
            _labelEndDateTime.Click += OnEndDateClick;
            _labelStartLocation.Click += OnStartLocationClick;
            _labelEndLocation.Click += OnEndLocationClick;
            _labelWaypoints.Click += OnWaypointsClick;
            _videoListView.ItemClick += OnVideoListItemClick;
            _videoButtonAdd.Click += OnVideoButtonAddClick;
            _videoButtonModify.Click += OnVideoButtonModifyClick;
            _videoButtonDelete.Click += OnVideoButtonDeleteClick;
            _videoButtonView.Click += OnVideoButtonViewClick;
            _buttonSubmit.Click += OnSubmitButtonClick;
        }

        //============================================================
        public override async void OnBackPressed()
        {
            var progressDialog = Utils.ShowProgressDialog(this, "Deleting temporary videos", "Please wait ...");
            await _viewPresenter.BackPressed();
            progressDialog.Dismiss();
        }

        //============================================================
        private void OnVideoListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _viewPresenter.ItemClick(e.Position, e.View);
        }

        //============================================================
        private async void OnVideoButtonAddClick(object sender, EventArgs e)
        {
            await _viewPresenter.VideoAddClick();
        }

        //============================================================
        private void OnVideoButtonModifyClick(object sender, EventArgs e)
        {

        }

        //============================================================
        private void OnVideoButtonDeleteClick(object sender, EventArgs e)
        {
            _viewPresenter.VideoDeleteClick();
        }

        //============================================================
        private void OnVideoButtonViewClick(object sender, EventArgs e)
        {
            _viewPresenter.VideoViewClick();
        }

        //============================================================
        private void OnStartDateClick(object sender, EventArgs e)
        {
            _viewPresenter.StartDateClick();
        }

        //============================================================
        private void OnEndDateClick(object sender, EventArgs e)
        {
            _viewPresenter.EndDateClick();
        }

        //============================================================
        private void OnStartLocationClick(object sender, EventArgs e)
        {
            _viewPresenter.StartLocationClick();
        }

        //============================================================
        private void OnEndLocationClick(object sender, EventArgs e)
        {
            _viewPresenter.EndLocationClick();
        }

        //============================================================
        private void OnWaypointsClick(object sender, EventArgs e)
        {
            _viewPresenter.WaypointsClick();
        }

        //============================================================
        private async void OnSubmitButtonClick(object sender, EventArgs e)
        {
            await _viewPresenter.SubmitClick(_textDescription.Text);
        }
    }
}