using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
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
using Constants = DrivingAssistant.AndroidApp.Tools.Constants;

namespace DrivingAssistant.AndroidApp.Activities.SessionEdit
{
    [Activity(Label = "SessionEditActivity")]
    public class SessionEditActivity : Activity
    {
        #region ActivityFields

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

        #endregion

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

            Session currentSession = null;

            if (Intent.HasExtra("session"))
            { 
                currentSession = JsonConvert.DeserializeObject<Session>(Intent.GetStringExtra("session")!);
                _textDescription.Text = currentSession.Name;
                _labelStartDateTime.Text = currentSession.StartDateTime.ToString(Constants.DateTimeFormat);
                _labelEndDateTime.Text = currentSession.EndDateTime.ToString(Constants.DateTimeFormat);
                _labelStartLocation.Text = currentSession.StartLocation.X + " " + currentSession.StartLocation.Y;
                _labelEndLocation.Text = currentSession.EndLocation.X + " " + currentSession.EndLocation.Y;
                _labelWaypoints.Text = currentSession.Waypoints.Count + " Selected";
                _videoListView.Adapter = new VideoThumbnailViewModelAdapter(this, (await (new VideoService()).GetVideoBySessionAsync(currentSession.Id)).ToList());
            }

            _viewPresenter = new SessionEditActivityViewPresenter(this, currentSession, _user, await Geolocation.GetLastKnownLocationAsync());
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;
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
                    _videoListView.Adapter = new VideoThumbnailViewModelAdapter(this, e.Data as List<Core.Models.Video>);
                    break;
                }
                case NotificationCommand.SessionEditActivity_VideoView:
                {
                    var intent = new Intent(this, typeof(VideoActivity));
                    intent.PutExtra("video", JsonConvert.SerializeObject(e.Data as Core.Models.Video));
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
                    var startLocation = e.Data as Point;
                    _labelStartLocation.Text = startLocation.X + " " + startLocation.Y;
                    break;
                }
                case NotificationCommand.SessionEditActivity_EndLocation:
                {
                    var endLocation = e.Data as Point;
                    _labelEndLocation.Text = endLocation.X + " " + endLocation.Y;
                    break;
                }
                case NotificationCommand.SessionEditActivity_Waypoints:
                {
                    var waypoints = e.Data as List<Point>;
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
            #region TextInputEditText

            _textDescription = FindViewById<TextInputEditText>(Resource.Id.sessionEditTextDescription);

            #endregion

            #region TextView

            _labelStartDateTime = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedStartDateTime);
            _labelEndDateTime = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedEndDateTime);
            _labelStartLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedStartPosition);
            _labelEndLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedEndPosition);
            _labelWaypoints = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedWaypoints);

            #endregion

            #region ListView

            _videoListView = FindViewById<ListView>(Resource.Id.sessionEditVideoList);

            #endregion

            #region Button

            _videoButtonAdd = FindViewById<Button>(Resource.Id.videosButtonAdd);
            _videoButtonModify = FindViewById<Button>(Resource.Id.videosButtonModify);
            _videoButtonDelete = FindViewById<Button>(Resource.Id.videosButtonDelete);
            _videoButtonView = FindViewById<Button>(Resource.Id.videosButtonView);
            _buttonSubmit = FindViewById<Button>(Resource.Id.sessionEditButtonSubmit);

            #endregion

            #region Events

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

            #endregion
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