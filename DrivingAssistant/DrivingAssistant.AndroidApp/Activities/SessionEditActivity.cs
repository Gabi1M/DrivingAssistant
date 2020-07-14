using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "SessionEditActivity")]
    public class SessionEditActivity : Activity
    {
        private TextView _labelDescription;
        private TextInputEditText _textDescription;

        private TextView _labelStartDateTime;
        private TextView _labelStartDateTimeValue;

        private TextView _labelEndDateTime;
        private TextView _labelEndDateTimeValue;

        private TextView _labelStartLocation;
        private TextView _labelStartLocationX;
        private TextView _labelStartLocationY;
        private EditText _textStartLocationX;
        private EditText _textStartLocationY;

        private TextView _labelEndLocation;
        private TextView _labelEndLocationX;
        private TextView _labelEndLocationY;
        private EditText _textEndLocationX;
        private EditText _textEndLocationY;

        private TextView _labelSelectedMedia;
        private Button _buttonSelectMedia;

        private Button _buttonSubmit;

        private User _user;
        private SessionService _sessionService;
        private MediaService _mediaService;

        private readonly List<Media> _selectedMedia = new List<Media>();
        private DateTime _selectedStartDateTime;
        private DateTime _selectedEndDateTime;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_session_edit);

            _sessionService = new SessionService("http://192.168.100.234:3287");
            _mediaService = new MediaService("http://192.168.100.234:3287");
            _user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("user"));

            _labelDescription = FindViewById<TextView>(Resource.Id.sessionEditLabelDescription);
            _textDescription = FindViewById<TextInputEditText>(Resource.Id.sessionEditTextDescription);
            _labelStartDateTime = FindViewById<TextView>(Resource.Id.sessionEditLabelStartDateTime);
            _labelStartDateTimeValue = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedStartDateTime);
            _labelEndDateTime = FindViewById<TextView>(Resource.Id.sessionEditLabelEndDateTime);
            _labelEndDateTimeValue = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedEndDateTime);
            _labelStartLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelStartPosition);
            _labelStartLocationX = FindViewById<TextView>(Resource.Id.sessionEditLabelStartX);
            _labelStartLocationY = FindViewById<TextView>(Resource.Id.sessionEditLabelStartY);
            _textStartLocationX = FindViewById<EditText>(Resource.Id.sessionEditTextStartX);
            _textStartLocationY = FindViewById<EditText>(Resource.Id.sessionEditTextStartY);
            _labelEndLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelEndPosition);
            _labelEndLocationX = FindViewById<TextView>(Resource.Id.sessionEditLabelEndX);
            _labelEndLocationY = FindViewById<TextView>(Resource.Id.sessionEditLabelEndY);
            _textEndLocationX = FindViewById<EditText>(Resource.Id.sessionEditTextEndX);
            _textEndLocationY = FindViewById<EditText>(Resource.Id.sessionEditTextEndY);
            _labelSelectedMedia = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedMedia);
            _buttonSelectMedia = FindViewById<Button>(Resource.Id.sessionEditButtonSelectMedia);
            _buttonSubmit = FindViewById<Button>(Resource.Id.sessionEditButtonSubmit);

            _buttonSelectMedia.Click += OnButtonSelectMediaClick;
            _buttonSubmit.Click += OnButtonSubmitClick;
            _labelStartDateTimeValue.Click += ButtonSelectStartDateOnClick;
            _labelEndDateTimeValue.Click += ButtonSelectEndDateOnClick;
        }

        //============================================================
        private void ButtonSelectStartDateOnClick(object sender, EventArgs e)
        {
            var datePickerDialog = new DatePickerDialog(this, (o, args) =>
            {
                _selectedStartDateTime = args.Date;
                var timePickerDialog = new TimePickerDialog(this, (sender1, eventArgs) =>
                {
                    _selectedStartDateTime = _selectedStartDateTime.AddHours(eventArgs.HourOfDay);
                    _selectedStartDateTime = _selectedStartDateTime.AddMinutes(eventArgs.Minute);
                    _labelStartDateTimeValue.Text = _selectedStartDateTime.ToString();
                }, DateTime.Now.Hour, DateTime.Now.Minute, true);
                timePickerDialog.Show();
            }, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            datePickerDialog.Show();
        }

        //============================================================
        private void ButtonSelectEndDateOnClick(object sender, EventArgs e)
        {
            var datePickerDialog = new DatePickerDialog(this, (o, args) =>
            {
                _selectedEndDateTime = args.Date;
                var timePickerDialog = new TimePickerDialog(this, (sender1, eventArgs) =>
                {
                    _selectedEndDateTime = _selectedEndDateTime.AddHours(eventArgs.HourOfDay);
                    _selectedEndDateTime = _selectedEndDateTime.AddMinutes(eventArgs.Minute);
                    _labelEndDateTimeValue.Text = _selectedEndDateTime.ToString();
                }, DateTime.Now.Hour, DateTime.Now.Minute, true);
                timePickerDialog.Show();
            }, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            datePickerDialog.Show();
        }

        //============================================================
        private async void OnButtonSelectMediaClick(object sender, EventArgs e)
        {
            var images = (await _mediaService.GetImagesAsync(_user.Id)).Where(x => !x.IsInSession()); 
            var videos = (await _mediaService.GetVideosAsync(_user.Id)).Where(x => !x.IsInSession());

            var medias = new List<Media>();
            medias.AddRange(images);
            medias.AddRange(videos);
            medias = medias.OrderBy(x => x.DateAdded).ToList();
            var mediasString = medias.Select(x => x.Type + ", " + x.DateAdded.ToString(CultureInfo.InvariantCulture)).ToArray();

            var selectedItems = new List<int>();
            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Choose media to include in this session");
            alert.SetMultiChoiceItems(mediasString, new bool[mediasString.Length], (o, args) =>
            {
                if (args.IsChecked == false && selectedItems.Contains(args.Which))
                {
                    selectedItems.Remove(args.Which);
                }

                if (args.IsChecked && !selectedItems.Contains(args.Which))
                {
                    selectedItems.Add(args.Which);
                }
            });

            alert.SetPositiveButton("Ok", (o, args) =>
            {
                _labelSelectedMedia.Text = "Selected " + selectedItems.Count + " items!";
                foreach (var selectedItem in selectedItems)
                {
                    _selectedMedia.Add(medias.ElementAt(selectedItem));
                }
            });

            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private async void OnButtonSubmitClick(object sender, EventArgs e)
        {
            if (_selectedMedia.Count == 0)
            {
                Toast.MakeText(this, "There is no media selected!", ToastLength.Short).Show();
                return;
            }

            if (_selectedStartDateTime == null || _selectedEndDateTime == null ||
                string.IsNullOrEmpty(_textDescription.Text) || string.IsNullOrEmpty(_textStartLocationX.Text) ||
                string.IsNullOrEmpty(_textStartLocationY.Text) || string.IsNullOrEmpty(_textEndLocationX.Text) ||
                string.IsNullOrEmpty(_textEndLocationY.Text))
            {
                Toast.MakeText(this, "Some fields were left blank!", ToastLength.Short).Show();
                return;
            }

            var session = new Session(_textDescription.Text.Trim(), _selectedStartDateTime, _selectedEndDateTime,
                new Coordinates(Convert.ToDecimal(_textStartLocationX.Text.Trim()),
                    Convert.ToDecimal(_textStartLocationY.Text.Trim())),
                new Coordinates(Convert.ToDecimal(_textEndLocationX.Text.Trim()),
                    Convert.ToDecimal(_textEndLocationY.Text.Trim())), default, _user.Id);
            session.Id = await _sessionService.SetAsync(session);
            foreach (var media in _selectedMedia)
            {
                media.SessionId = session.Id;
                if (media.Type == MediaType.Image)
                {
                    await _mediaService.UpdateImageAsync(media);
                }
                else if (media.Type == MediaType.Video)
                {
                    await _mediaService.UpdateVideoAsync(media);
                }
            }

            Finish();
        }
    }
}