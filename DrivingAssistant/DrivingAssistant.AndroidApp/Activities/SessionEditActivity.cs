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
        private TextView _labelWaypoints;
        private ListView _videoListView;
        private Button _videoButtonAdd;
        private Button _videoButtonModify;
        private Button _videoButtonDelete;
        private Button _videoButtonView;
        private Button _buttonSubmit;

        #endregion

        #region Service

        private readonly SessionService _sessionService = new SessionService();
        private readonly VideoService _videoService = new VideoService();

        #endregion

        private User _user;
        private Session _currentSession;
        private bool _newSession;

        private ICollection<Video> _videoList = new List<Video>();

        private DateTime? _selectedStartDateTime;
        private DateTime? _selectedEndDateTime;
        private Point _selectedStartPoint;
        private Point _selectedEndPoint;
        private Location _currentLocation;
        private ICollection<Point> _selectedWaypoints = new List<Point>();

        private int _selectedVideoPosition = -1;
        private View _selectedVideoView;

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
                _videoList = (await _videoService.GetVideoBySessionAsync(_currentSession.Id)).ToList();
                _textDescription.Text = _currentSession.Name;
                _selectedStartDateTime = _currentSession.StartDateTime;
                _selectedEndDateTime = _currentSession.EndDateTime;
                _selectedStartPoint = _currentSession.StartLocation;
                _selectedEndPoint = _currentSession.EndLocation;
                _selectedWaypoints = _currentSession.Waypoints;
                _labelStartDateTime.Text = _selectedStartDateTime?.ToString(Constants.DateTimeFormat);
                _labelEndDateTime.Text = _selectedEndDateTime?.ToString(Constants.DateTimeFormat);
                _labelStartLocation.Text = _selectedStartPoint.X + " " + _selectedStartPoint.Y;
                _labelEndLocation.Text = _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                _labelWaypoints.Text = _selectedWaypoints.Count + " Selected";
                _videoListView.Adapter = new VideoThumbnailViewModelAdapter(this, _videoList);
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
            _videoButtonDelete.Click += OnVideoButtonDeleteClick;
            _videoButtonView.Click += OnVideoButtonViewClick;
            _buttonSubmit.Click += OnSubmitButtonClick;

            #endregion
        }

        //============================================================
        public override async void OnBackPressed()
        {
            var progressDialog = ProgressDialog.Show(this, "Deleting temporary videos", "Please wait...");
            foreach (var video in _videoList.Where(x => !x.IsInSession()))
            {
                await _videoService.DeleteVideoAsync(video.Id);
            }
            progressDialog.Dismiss();
            base.OnBackPressed();
        }

        //============================================================
        private async Task RefreshVideoSource(bool fetchRemote = false)
        {
            if (fetchRemote)
            {
                _videoList = (await _videoService.GetVideoBySessionAsync(_currentSession.Id)).ToList();
            }

            _videoListView.Adapter?.Dispose();
            _videoListView.Adapter = new VideoThumbnailViewModelAdapter(this, _videoList);
        }

        //============================================================
        private void OnVideoListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedVideoView?.SetBackgroundResource(0);
            e.View.SetBackgroundResource(Resource.Drawable.list_element_border);
            _selectedVideoPosition = e.Position;
            _selectedVideoView = e.View;
        }

        //============================================================
        private async void OnVideoButtonAddClick(object sender, EventArgs e)
        {
            var filedata = await CrossFilePicker.Current.PickFile();
            if (filedata == null)
            {
                return;
            }

            if (Path.GetExtension(filedata.FilePath) != ".jpg" && Path.GetExtension(filedata.FilePath) != ".mp4")
            {
                Toast.MakeText(this, "Selected file is not a MP4 video file!", ToastLength.Short).Show();
                return;
            }

            var progressDialog = ProgressDialog.Show(this, "Video Upload", "Uploading...");
            var videoId = await _videoService.SetVideoStreamAsync(filedata.GetStream());
            var video = await _videoService.GetVideoByIdAsync(videoId);
            if (_newSession)
            {
                _videoList.Add(video);
                await RefreshVideoSource();
            }
            else
            {
                video.SessionId = _currentSession.Id;
                await _videoService.UpdateVideoAsync(video);
                await RefreshVideoSource(true);
            }
            progressDialog.Dismiss();
        }

        //============================================================
        private void OnVideoButtonDeleteClick(object sender, EventArgs e)
        {
            if (_selectedVideoPosition == -1)
            {
                Toast.MakeText(this, "No video selected!", ToastLength.Short).Show();
                return;
            }

            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Confirm Delete");
            alert.SetMessage("Action cannot be undone");
            alert.SetPositiveButton("Delete", async (o, args) =>
            {
                var video = _videoList.ElementAt(_selectedVideoPosition);
                await _videoService.DeleteVideoAsync(video.Id);

                if (_newSession)
                {
                    _videoList.Remove(video);
                    await RefreshVideoSource();
                }
                else
                {
                    await RefreshVideoSource(true);
                }
            });

            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void OnVideoButtonViewClick(object sender, EventArgs e)
        {
            if (_selectedVideoPosition == -1)
            {
                Toast.MakeText(this, "No video selected!", ToastLength.Short).Show();
                return;
            }

            var video = _videoList.ElementAt(_selectedVideoPosition);
            var intent = new Intent(this, typeof(VideoActivity));
            intent.PutExtra("video", JsonConvert.SerializeObject(video));
            StartActivity(intent);
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
                    _labelStartDateTime.Text = _selectedStartDateTime?.ToString(Constants.DateTimeFormat);
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
                    _labelEndDateTime.Text = _selectedEndDateTime?.ToString(Constants.DateTimeFormat);
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
                    points.AddRange(_selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
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
                    _labelStartLocation.Text = _selectedStartPoint.X + " " + _selectedStartPoint.Y;
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
                    points.AddRange(_selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
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
                    _labelEndLocation.Text = _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                }
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create().Show();
        }

        //============================================================
        private void OnWaypointsClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(this);
            var view = LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
            alert.SetView(view);
            mapControl.Map = SetupMap();
            mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(_currentLocation.Longitude, _currentLocation.Latitude), mapControl.Map.Resolutions[9]);
            var selectedWaypoints = new List<Point>(_selectedWaypoints!);
            mapControl.Info += (o, args) =>
            {
                var zoomWidgetEnvelope = mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                if (!zoomWidgetEnvelope.Contains(args.MapInfo.ScreenPosition))
                {
                    if (mapControl.Map.Layers.Any(x => x.Name == "Waypoints"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.FindLayer("Waypoints").First());
                    }

                    if (mapControl.Map.Layers.Any(x => x.Name == "Line"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.First(x => x.Name == "Line"));
                    }

                    var tempPoint = SphericalMercator.ToLonLat(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y);

                    if (args.MapInfo.Feature != null && args.MapInfo.Layer.Name == "Waypoints")
                    {
                        var featurePoint = args.MapInfo.Feature.Geometry.AllVertices().First();
                        var point = selectedWaypoints.First(x => featurePoint.Equals(SphericalMercator.FromLonLat(x.X, x.Y)));
                        selectedWaypoints.Remove(point);
                    }
                    else
                    {
                        selectedWaypoints.Add(new Point(tempPoint.X, tempPoint.Y));
                    }

                    var points = new List<Mapsui.Geometries.Point>();
                    if (_selectedStartPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedStartPoint.X, _selectedStartPoint.Y));
                    }
                    points.AddRange(selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
                    if (_selectedEndPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedEndPoint.X, _selectedEndPoint.Y));
                    }
                    mapControl.Map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(MapTools.CreatePointLayer("Waypoints", Color.Yellow, 0.5, selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
                }
            };
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedWaypoints = new List<Point>(selectedWaypoints);
                _labelWaypoints.Text = _selectedWaypoints.Count + " Selected";
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create().Show();
        }

        //============================================================
        private async void OnSubmitButtonClick(object sender, EventArgs e)
        {
            if (!_newSession)
            {
                _currentSession.Name = _textDescription.Text;
                _currentSession.StartDateTime = _selectedStartDateTime ?? DateTime.Now;
                _currentSession.EndDateTime = _selectedStartDateTime ?? DateTime.Now;
                _currentSession.StartLocation = _selectedStartPoint;
                _currentSession.EndLocation = _selectedEndPoint;
                _currentSession.Waypoints = _selectedWaypoints;
                await _sessionService.SetAsync(_currentSession);
                foreach (var video in _videoList.Where(x => x.SessionId != _currentSession.Id))
                {
                    video.SessionId = _currentSession.Id;
                    await _videoService.UpdateVideoAsync(video);
                }
            }
            else
            {
                _currentSession = new Session
                {
                    Name = !string.IsNullOrEmpty(_textDescription.Text) ? _textDescription.Text.Trim() : string.Empty,
                    StartDateTime = _selectedStartDateTime ?? DateTime.Now,
                    EndDateTime = _selectedEndDateTime ?? DateTime.Now,
                    StartLocation = _selectedStartPoint ?? new Point(0,0),
                    EndLocation = _selectedEndPoint ?? new Point(0,0),
                    Waypoints = _selectedWaypoints,
                    Id = -1,
                    Status = SessionStatus.Unprocessed,
                    UserId = _user.Id,
                    DateAdded = DateTime.Now
                };
                _currentSession.Id = await _sessionService.SetAsync(_currentSession);
                foreach (var video in _videoList)
                {
                    video.SessionId = _currentSession.Id;
                    await _videoService.UpdateVideoAsync(video);
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

            if (_selectedStartPoint != null || _selectedEndPoint != null || (_selectedWaypoints != null && _selectedWaypoints.Count != 0))
            {
                var points = new List<Mapsui.Geometries.Point>();
                if (_selectedStartPoint != null)
                {
                    points.Add(SphericalMercator.FromLonLat(_selectedStartPoint.X, _selectedStartPoint.Y));
                }

                if (_selectedWaypoints != null && _selectedWaypoints.Count != 0)
                {
                    points.AddRange(_selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
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

            if (_selectedWaypoints != null && _selectedWaypoints.Count != 0)
            {
                map.Layers.Add(MapTools.CreatePointLayer("Waypoints", Color.Yellow, 0.5, _selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
            }

            return map;
        }
    }
}