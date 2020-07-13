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

        private List<Media> _selectedMedia = new List<Media>();

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
        }

        //============================================================
        private async void OnButtonSelectMediaClick(object sender, EventArgs e)
        {
            var images = await _mediaService.GetImagesAsync(_user.Id); 
            var videos = await _mediaService.GetVideosAsync(_user.Id);

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

            var session = new Session(_textDescription.Text.Trim(), DateTime.Now, DateTime.Now,
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