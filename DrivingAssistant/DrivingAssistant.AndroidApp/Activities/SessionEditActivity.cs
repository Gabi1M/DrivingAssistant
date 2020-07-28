using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Adapters.ViewModelAdapters;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Mapsui;
using Mapsui.Projection;
using Mapsui.Styles;
using Mapsui.UI.Android;
using Mapsui.Utilities;
using Mapsui.Widgets.Zoom;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Xamarin.Essentials;
using Constants = DrivingAssistant.AndroidApp.Tools.Constants;

namespace DrivingAssistant.AndroidApp.Activities
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
        private TextView _labelInterLocations;
        private ListView _mediaListView;
        private Button _mediaButtonAdd;
        private Button _mediaButtonModify;
        private Button _mediaButtonDelete;
        private Button _mediaButtonView;
        private Button _buttonSubmit;

        #endregion

        #region Service

        private readonly SessionService _sessionService = new SessionService();
        private readonly MediaService _mediaService = new MediaService();

        #endregion

        private User _user;
        private Session _currentSession;
        private bool _newSession;

        private ICollection<Media> _mediaList = new List<Media>();

        private DateTime? _selectedStartDateTime;
        private DateTime? _selectedEndDateTime;
        private Point _selectedStartPoint;
        private Point _selectedEndPoint;
        private Location _currentLocation;
        private ICollection<Point> _selectedIntermediaries = new List<Point>();

        private int _selectedMediaPosition = -1;
        private View _selectedMediaView;

        //============================================================
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_session_edit);
            SetupActivityFields();

            _user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("user"));
            _newSession = !Intent.HasExtra("session");
            _currentLocation = await Geolocation.GetLastKnownLocationAsync();

            if (!_newSession)
            {
                _currentSession = JsonConvert.DeserializeObject<Session>(Intent.GetStringExtra("session"));
                _mediaList = (await _mediaService.GetMediaBySessionAsync(_currentSession.Id)).ToList();
                _textDescription.Text = _currentSession.Name;
                _selectedStartDateTime = _currentSession.StartDateTime;
                _selectedEndDateTime = _currentSession.EndDateTime;
                _selectedStartPoint = _currentSession.StartPoint;
                _selectedEndPoint = _currentSession.EndPoint;
                _selectedIntermediaries = _currentSession.IntermediatePoints;
                _labelStartDateTime.Text = "Start Date: " + _selectedStartDateTime?.ToString(Constants.DateTimeFormat);
                _labelEndDateTime.Text = "End Date: " + _selectedEndDateTime?.ToString(Constants.DateTimeFormat);
                _labelStartLocation.Text = "Start Location: " + _selectedStartPoint.X + " " + _selectedStartPoint.Y;
                _labelEndLocation.Text = "End Location: " + _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                _labelInterLocations.Text = "Selected " + _selectedIntermediaries.Count + " Intermediate Points";
                _mediaListView.Adapter = new MediaThumbnailViewModelAdapter(this, _mediaList);
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
            _labelInterLocations = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedIntermediaries);

            #endregion

            #region ListView

            _mediaListView = FindViewById<ListView>(Resource.Id.sessionEditMediaList);

            #endregion

            #region Button

            _mediaButtonAdd = FindViewById<Button>(Resource.Id.mediasButtonAdd);
            _mediaButtonModify = FindViewById<Button>(Resource.Id.mediasButtonModify);
            _mediaButtonDelete = FindViewById<Button>(Resource.Id.mediasButtonDelete);
            _mediaButtonView = FindViewById<Button>(Resource.Id.mediasButtonView);
            _buttonSubmit = FindViewById<Button>(Resource.Id.sessionEditButtonSubmit);

            #endregion

            #region Events

            _labelStartDateTime.Click += OnStartDateClick;
            _labelEndDateTime.Click += OnEndDateClick;
            _labelStartLocation.Click += OnStartLocationClick;
            _labelEndLocation.Click += OnEndLocationClick;
            _labelInterLocations.Click += OnInterLocationsClick;
            _mediaListView.ItemClick += OnMediaListItemClick;
            _mediaButtonAdd.Click += OnMediaButtonAddClick;
            _mediaButtonDelete.Click += OnMediaButtonDeleteClick;
            _mediaButtonView.Click += OnMediaButtonViewClick;
            _buttonSubmit.Click += OnSubmitButtonClick;

            #endregion
        }

        //============================================================
        public override async void OnBackPressed()
        {
            var progressDialog = ProgressDialog.Show(this, "Deleting temporary media", "Please wait...");
            foreach (var media in _mediaList.Where(x => !x.IsInSession()))
            {
                await _mediaService.DeleteMediaAsync(media.Id);
            }
            progressDialog.Dismiss();
            base.OnBackPressed();
        }

        //============================================================
        private async Task RefreshMediaSource(bool fetchRemote = false)
        {
            if (fetchRemote)
            {
                _mediaList = (await _mediaService.GetMediaBySessionAsync(_currentSession.Id)).ToList();
            }

            _mediaListView.Adapter?.Dispose();
            _mediaListView.Adapter = new MediaThumbnailViewModelAdapter(this, _mediaList);
        }

        //============================================================
        private void OnMediaListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedMediaView?.SetBackgroundResource(0);
            e.View.SetBackgroundResource(Resource.Drawable.list_element_border);
            _selectedMediaPosition = e.Position;
            _selectedMediaView = e.View;
        }

        //============================================================
        private async void OnMediaButtonAddClick(object sender, EventArgs e)
        {
            var filedata = await CrossFilePicker.Current.PickFile();
            if (filedata == null)
            {
                return;
            }

            if (Path.GetExtension(filedata.FilePath) != ".jpg" && Path.GetExtension(filedata.FilePath) != ".mp4")
            {
                Toast.MakeText(this, "Selected file is not a Jpeg image file or MP4 video file!", ToastLength.Short).Show();
                return;
            }

            var mediaType = Path.GetExtension(filedata.FilePath) == ".jpg" ? MediaType.Image : MediaType.Video;
            var progressDialog = ProgressDialog.Show(this, mediaType == MediaType.Image ? "Image Upload" : "Video Upload", "Uploading...");
            var mediaId = await _mediaService.SetMediaStreamAsync(filedata.GetStream(), mediaType);
            var media = await _mediaService.GetMediaByIdAsync(mediaId);
            if (_newSession)
            {
                _mediaList.Add(media);
                await RefreshMediaSource();
            }
            else
            {
                media.SessionId = _currentSession.Id;
                await _mediaService.UpdateMediaAsync(media);
                await RefreshMediaSource(true);
            }
            progressDialog.Dismiss();
        }

        //============================================================
        private void OnMediaButtonDeleteClick(object sender, EventArgs e)
        {
            if (_selectedMediaPosition == -1)
            {
                Toast.MakeText(this, "No media selected!", ToastLength.Short).Show();
                return;
            }

            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Confirm Delete");
            alert.SetMessage("Action cannot be undone");
            alert.SetPositiveButton("Delete", async (o, args) =>
            {
                var media = _mediaList.ElementAt(_selectedMediaPosition);
                await _mediaService.DeleteMediaAsync(media.Id);

                if (_newSession)
                {
                    _mediaList.Remove(media);
                    await RefreshMediaSource();
                }
                else
                {
                    await RefreshMediaSource(true);
                }
            });

            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void OnMediaButtonViewClick(object sender, EventArgs e)
        {
            if (_selectedMediaPosition == -1)
            {
                Toast.MakeText(this, "No media selected!", ToastLength.Short).Show();
                return;
            }

            var media = _mediaList.ElementAt(_selectedMediaPosition);
            if (media.Type == MediaType.Image)
            {
                var intent = new Intent(this, typeof(GalleryActivity));
                intent.PutExtra("image", JsonConvert.SerializeObject(media));
                StartActivity(intent);
            }
            else if (media.Type == MediaType.Video)
            {
                var intent = new Intent(this, typeof(VideoActivity));
                intent.PutExtra("video", JsonConvert.SerializeObject(media));
                StartActivity(intent);
            }
        }

        //============================================================
        private void OnStartDateClick(object sender, EventArgs e)
        {
            new DatePickerDialog(this, (o, args) =>
            {
                _selectedStartDateTime = args.Date;
                new TimePickerDialog(this, (sender1, eventArgs) =>
                {
                    _selectedStartDateTime = _selectedStartDateTime?.AddHours(eventArgs.HourOfDay);
                    _selectedStartDateTime = _selectedStartDateTime?.AddMinutes(eventArgs.Minute);
                    _labelStartDateTime.Text = "Start Date: " + _selectedStartDateTime?.ToString(Constants.DateTimeFormat);
                }, DateTime.Now.Hour, DateTime.Now.Minute, true).Show();
            }, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Show();
        }

        //============================================================
        private void OnEndDateClick(object sender, EventArgs e)
        {
            new DatePickerDialog(this, (o, args) =>
            {
                _selectedEndDateTime = args.Date;
                new TimePickerDialog(this, (sender1, eventArgs) =>
                {
                    _selectedEndDateTime = _selectedEndDateTime?.AddHours(eventArgs.HourOfDay);
                    _selectedEndDateTime = _selectedEndDateTime?.AddMinutes(eventArgs.Minute);
                    _labelEndDateTime.Text = "End Date: " + _selectedEndDateTime?.ToString(Constants.DateTimeFormat);
                }, DateTime.Now.Hour, DateTime.Now.Minute, true).Show();
            }, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Show();
        }

        //============================================================
        private void OnStartLocationClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(this);
            var view = LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
            alert.SetView(view);
            mapControl.Map = SetupMap();
            mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(_currentLocation.Longitude, _currentLocation.Latitude), mapControl.Map.Resolutions[9]);
            var selectedStartPoint = _selectedStartPoint;
            mapControl.Info += (o, args) =>
            {
                var zoomWidgetEnvelope = mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                if (!zoomWidgetEnvelope.Contains(args.MapInfo.ScreenPosition))
                {
                    if (mapControl.Map.Layers.Any(x => x.Name == "StartPoint"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.First(x => x.Name == "StartPoint"));
                    }

                    if (mapControl.Map.Layers.Any(x => x.Name == "Line"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.First(x => x.Name == "Line"));
                    }

                    var tempPoint = SphericalMercator.ToLonLat(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y);
                    selectedStartPoint = new Point(tempPoint.X, tempPoint.Y);

                    var points = new List<Mapsui.Geometries.Point>
                    {
                        new Mapsui.Geometries.Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)
                    };
                    points.AddRange(_selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
                    if (_selectedEndPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedEndPoint.X, _selectedEndPoint.Y));
                    }
                    mapControl.Map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(MapTools.CreatePointLayer("StartPoint", Color.Green, 0.5, new Mapsui.Geometries.Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                }
            };
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedStartPoint = selectedStartPoint;
                if (_selectedStartPoint != null)
                {
                    _labelStartLocation.Text = "Start Location: " + _selectedStartPoint.X + " " + _selectedStartPoint.Y;
                }
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create().Show();
        }

        //============================================================
        private void OnEndLocationClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(this);
            var view = LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
            alert.SetView(view);
            mapControl.Map = SetupMap();
            mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(_currentLocation.Longitude, _currentLocation.Latitude), mapControl.Map.Resolutions[9]);
            var selectedEndPoint = _selectedEndPoint;
            mapControl.Info += (o, args) =>
            {
                var zoomWidgetEnvelope = mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                if (!zoomWidgetEnvelope.Contains(args.MapInfo.ScreenPosition))
                {
                    if (mapControl.Map.Layers.Any(x => x.Name == "EndPoint"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.FindLayer("EndPoint").First());
                    }

                    if (mapControl.Map.Layers.Any(x => x.Name == "Line"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.First(x => x.Name == "Line"));
                    }

                    var tempPoint = SphericalMercator.ToLonLat(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y);
                    selectedEndPoint = new Point(tempPoint.X, tempPoint.Y);

                    var points = new List<Mapsui.Geometries.Point>();
                    if (_selectedStartPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedStartPoint.X, _selectedStartPoint.Y));
                    }
                    points.AddRange(_selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
                    points.Add(new Mapsui.Geometries.Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y));
                    mapControl.Map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(MapTools.CreatePointLayer("EndPoint", Color.Red, 0.5, new Mapsui.Geometries.Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                }
            };
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedEndPoint = selectedEndPoint;
                if (_selectedEndPoint != null)
                {
                    _labelEndLocation.Text = "End Location: " + _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                }
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create().Show();
        }

        //============================================================
        private void OnInterLocationsClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(this);
            var view = LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
            alert.SetView(view);
            mapControl.Map = SetupMap();
            mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(_currentLocation.Longitude, _currentLocation.Latitude), mapControl.Map.Resolutions[9]);
            var selectedIntermediaries = new List<Point>(_selectedIntermediaries!);
            mapControl.Info += (o, args) =>
            {
                var zoomWidgetEnvelope = mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                if (!zoomWidgetEnvelope.Contains(args.MapInfo.ScreenPosition))
                {
                    if (mapControl.Map.Layers.Any(x => x.Name == "Intermediaries"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.FindLayer("Intermediaries").First());
                    }

                    if (mapControl.Map.Layers.Any(x => x.Name == "Line"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.First(x => x.Name == "Line"));
                    }

                    var tempPoint = SphericalMercator.ToLonLat(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y);

                    if (args.MapInfo.Feature != null && args.MapInfo.Layer.Name == "Intermediaries")
                    {
                        var featurePoint = args.MapInfo.Feature.Geometry.AllVertices().First();
                        var point = selectedIntermediaries.First(x => featurePoint.Equals(SphericalMercator.FromLonLat(x.X, x.Y)));
                        selectedIntermediaries.Remove(point);
                    }
                    else
                    {
                        selectedIntermediaries.Add(new Point(tempPoint.X, tempPoint.Y));
                    }

                    var points = new List<Mapsui.Geometries.Point>();
                    if (_selectedStartPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedStartPoint.X, _selectedStartPoint.Y));
                    }
                    points.AddRange(selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
                    if (_selectedEndPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedEndPoint.X, _selectedEndPoint.Y));
                    }
                    mapControl.Map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(MapTools.CreatePointLayer("Intermediaries", Color.Yellow, 0.5, selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
                }
            };
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedIntermediaries = new List<Point>(selectedIntermediaries);
                _labelInterLocations.Text = "Selected " + _selectedIntermediaries.Count + " Intermediate Points";
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create().Show();
        }

        //============================================================
        private async void OnSubmitButtonClick(object sender, EventArgs e)
        {
            /*if (_mediaList.Count == 0)
            {
                Toast.MakeText(this, "There is no media selected!", ToastLength.Short).Show();
                return;
            }

            if (_selectedStartDateTime == null || _selectedEndDateTime == null ||
                string.IsNullOrEmpty(_textDescription.Text) || _selectedStartPoint == null ||
                _selectedEndPoint == null || _selectedIntermediaries.Count == 0)
            {
                Toast.MakeText(this, "Some fields were left blank!", ToastLength.Short).Show();
                return;
            }*/

            if (!_newSession)
            {
                await _sessionService.SetAsync(_currentSession);
                foreach (var media in _mediaList.Where(x => x.SessionId != _currentSession.Id))
                {
                    media.SessionId = _currentSession.Id;
                    await _mediaService.UpdateMediaAsync(media);
                }
            }
            else
            {
                _currentSession = new Session
                {
                    Name = string.IsNullOrEmpty(_textDescription.Text) ? _textDescription.Text.Trim() : string.Empty,
                    StartDateTime = _selectedStartDateTime ?? DateTime.Now,
                    EndDateTime = _selectedEndDateTime ?? DateTime.Now,
                    StartPoint = _selectedStartPoint ?? new Point(0,0),
                    EndPoint = _selectedEndPoint ?? new Point(0,0),
                    IntermediatePoints = _selectedIntermediaries,
                    Id = -1,
                    Processed = false,
                    UserId = _user.Id,
                    DateAdded = DateTime.Now
                };
                _currentSession.Id = await _sessionService.SetAsync(_currentSession);
                foreach (var media in _mediaList)
                {
                    media.SessionId = _currentSession.Id;
                    await _mediaService.UpdateMediaAsync(media);
                }
            }

            Finish();
        }

        //============================================================
        private Mapsui.Map SetupMap()
        {
            var map = new Mapsui.Map()
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation(),
                Layers =
                {
                    OpenStreetMap.CreateTileLayer(),
                    MapTools.CreatePointLayer("CurrentLocation", Color.Blue, 0.5, SphericalMercator.FromLonLat(_currentLocation.Longitude, _currentLocation.Latitude))
                },
                Widgets =
                {
                    new ZoomInOutWidget()
                    {
                        MarginX = 20,
                        MarginY = 20
                    }
                }
            };

            if (_selectedStartPoint != null || _selectedEndPoint != null || (_selectedIntermediaries != null && _selectedIntermediaries.Count != 0))
            {
                var points = new List<Mapsui.Geometries.Point>();
                if (_selectedStartPoint != null)
                {
                    points.Add(SphericalMercator.FromLonLat(_selectedStartPoint.X, _selectedStartPoint.Y));
                }

                if (_selectedIntermediaries != null && _selectedIntermediaries.Count != 0)
                {
                    points.AddRange(_selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
                }

                if (_selectedEndPoint != null)
                {
                    points.Add(SphericalMercator.FromLonLat(_selectedEndPoint.X, _selectedEndPoint.Y));
                }
                map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
            }

            if (_selectedEndPoint != null)
            {
                map.Layers.Add(MapTools.CreatePointLayer("EndPoint", Color.Red, 0.5, SphericalMercator.FromLonLat(_selectedEndPoint.X, _selectedEndPoint.Y)));
            }

            if (_selectedStartPoint != null)
            {
                map.Layers.Add(MapTools.CreatePointLayer("StartPoint", Color.Green, 0.5, SphericalMercator.FromLonLat(_selectedStartPoint.X, _selectedStartPoint.Y)));
            }

            if (_selectedIntermediaries != null && _selectedIntermediaries.Count != 0)
            {
                map.Layers.Add(MapTools.CreatePointLayer("Intermediaries", Color.Yellow, 0.5, _selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
            }

            return map;
        }
    }
}