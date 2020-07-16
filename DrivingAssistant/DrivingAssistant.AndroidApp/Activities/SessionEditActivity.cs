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
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI;
using Mapsui.UI.Android;
using Mapsui.Utilities;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using Newtonsoft.Json;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "SessionEditActivity")]
    public class SessionEditActivity : Activity
    {
        private TextInputEditText _textDescription;
        private TextView _labelStartDateTimeValue;
        private TextView _labelEndDateTimeValue;
        private TextView _labelSelectedStartLocation;
        private TextView _labelSelectedEndLocation;
        private TextView _labelSelectedMedia;
        private Button _buttonSelectMedia;
        private Button _buttonSubmit;

        private User _user;
        private SessionService _sessionService;
        private MediaService _mediaService;

        private readonly List<Media> _selectedMedia = new List<Media>();
        private DateTime _selectedStartDateTime;
        private DateTime _selectedEndDateTime;
        private Point _selectedStartPoint;
        private Point _selectedEndPoint;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_session_edit);

            _sessionService = new SessionService("http://192.168.100.234:3287");
            _mediaService = new MediaService("http://192.168.100.234:3287");
            _user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("user"));

            _textDescription = FindViewById<TextInputEditText>(Resource.Id.sessionEditTextDescription);
            _labelStartDateTimeValue = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedStartDateTime);
            _labelEndDateTimeValue = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedEndDateTime);
            _labelSelectedStartLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedStartPosition);
            _labelSelectedEndLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedEndPosition);
            _labelSelectedMedia = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedMedia);
            _buttonSelectMedia = FindViewById<Button>(Resource.Id.sessionEditButtonSelectMedia);
            _buttonSubmit = FindViewById<Button>(Resource.Id.sessionEditButtonSubmit);

            if (Intent.HasExtra("session"))
            {
                var session = JsonConvert.DeserializeObject<Session>(Intent.GetStringExtra("session"));
                _textDescription.Text = session.Description;
                _selectedStartDateTime = session.StartDateTime;
                _selectedEndDateTime = session.EndDateTime;
                _selectedStartPoint = new Point(session.StartCoordinates.Latitude, session.StartCoordinates.Longitude);
                _selectedEndPoint = new Point(session.EndCoordinates.Latitude, session.EndCoordinates.Longitude);
                _labelStartDateTimeValue.Text = _selectedStartDateTime.ToString();
                _labelEndDateTimeValue.Text = _selectedEndDateTime.ToString();
                _labelSelectedStartLocation.Text = _selectedStartPoint.X + " " + _selectedStartPoint.Y;
                _labelEndDateTimeValue.Text = _selectedEndPoint.X + " " + _selectedEndPoint.Y;
            }

            _buttonSelectMedia.Click += OnButtonSelectMediaClick;
            _buttonSubmit.Click += OnButtonSubmitClick;
            _labelStartDateTimeValue.Click += ButtonSelectStartDateOnClick;
            _labelEndDateTimeValue.Click += ButtonSelectEndDateOnClick;
            _labelSelectedStartLocation.Click += LabelSelectedStartLocationOnClick;
            _labelSelectedEndLocation.Click += LabelSelectedEndLocationOnClick;
        }

        //============================================================
        private void LabelSelectedEndLocationOnClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(this);
            var view = LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
            var map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Widgets.Add(new ScaleBarWidget(map)
            {
                TextAlignment = Alignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            });
            map.Widgets.Add(new ZoomInOutWidget()
            {
                MarginX = 20,
                MarginY = 20
            });
            mapControl.Map = map;
            map.Home = n => n.NavigateTo(SphericalMercator.FromLonLat(23.598892, 46.765887), map.Resolutions[9]);
            mapControl.Info += (o, args) =>
            {
                var zoomWidgetEnvelope = mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                if (!zoomWidgetEnvelope.Contains(args.MapInfo.ScreenPosition))
                {
                    if (mapControl.Map.Layers.Any(x => x.Name == "Points"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.FindLayer("Points").First());
                    }
                    mapControl.Map.Layers.Add(CreatePointLayer(new Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                    _selectedEndPoint = SphericalMercator.ToLonLat(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y);
                    _labelSelectedEndLocation.Text = _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                }
            };
            alert.SetView(view);
            alert.SetPositiveButton("Ok", (o, args) => { });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void LabelSelectedStartLocationOnClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(this);
            var view = LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
            var map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Widgets.Add(new ScaleBarWidget(map)
            {
                TextAlignment = Alignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top
            });
            map.Widgets.Add(new ZoomInOutWidget()
            {
                MarginX = 20,
                MarginY = 20
            });
            mapControl.Map = map;
            map.Home = n => n.NavigateTo(SphericalMercator.FromLonLat(23.598892, 46.765887), map.Resolutions[9]);
            mapControl.Info += (o, args) =>
            {
                var zoomWidgetEnvelope = mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                if (!zoomWidgetEnvelope.Contains(args.MapInfo.ScreenPosition))
                {
                    if (mapControl.Map.Layers.Any(x => x.Name == "Points"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.FindLayer("Points").First());
                    }
                    mapControl.Map.Layers.Add(CreatePointLayer(new Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                    _selectedStartPoint = SphericalMercator.ToLonLat(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y);
                    _labelSelectedStartLocation.Text = _selectedStartPoint.X + " " + _selectedStartPoint.Y;
                }
            };
            alert.SetView(view);
            alert.SetPositiveButton("Ok", (o, args) => { });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private static MemoryLayer CreatePointLayer(params Point[] points)
        {
            return new MemoryLayer
            {
                Name = "Points",
                IsMapInfoLayer = true,
                DataSource = new MemoryProvider(points),
                Style = new SymbolStyle
                {
                    Fill = new Brush(Color.Red),
                    SymbolScale = 0.5
                }
            };
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
                string.IsNullOrEmpty(_textDescription.Text) || _selectedStartPoint == null ||
                _selectedEndPoint == null)
            {
                Toast.MakeText(this, "Some fields were left blank!", ToastLength.Short).Show();
                return;
            }

            var session = new Session(_textDescription.Text.Trim(), _selectedStartDateTime, _selectedEndDateTime,
                new Coordinates(Convert.ToSingle(_selectedStartPoint.X),
                    Convert.ToSingle(_selectedStartPoint.Y)),
                new Coordinates(Convert.ToSingle(_selectedEndPoint.X),
                    Convert.ToSingle(_selectedEndPoint.Y)), default, _user.Id);
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