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
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Android;
using Mapsui.Utilities;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using Newtonsoft.Json;
using Plugin.FilePicker;
using Xamarin.Essentials;
using Constants = DrivingAssistant.AndroidApp.Tools.Constants;
using Map = Mapsui.Map;
using Point = DrivingAssistant.Core.Models.Point;

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
        private TextView _labelSelectedIntermediaries;
        private ListView _mediaListView;
        private Button _mediaButtonAdd;
        private Button _mediaButtonModify;
        private Button _mediaButtonDelete;
        private Button _mediaButtonView;
        private Button _buttonSubmit;

        private User _user;
        private Session _session;
        private SessionService _sessionService;
        private MediaService _mediaService;
        private Location _currentLocation;

        private List<Media> _mediaList = new List<Media>();
        private DateTime? _selectedStartDateTime;
        private DateTime? _selectedEndDateTime;
        private Point _selectedStartPoint;
        private Point _selectedEndPoint;
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

            _sessionService = new SessionService(Constants.ServerUri);
            _mediaService = new MediaService(Constants.ServerUri);
            _user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("user"));
            _currentLocation = await Geolocation.GetLocationAsync();

            if (Intent.HasExtra("session"))
            {
                _session = JsonConvert.DeserializeObject<Session>(Intent.GetStringExtra("session"));
                _mediaList = (await _mediaService.GetMediaAsync(_user.Id)).Where(x => x.SessionId == _session.Id).ToList();
                _textDescription.Text = _session.Description;
                _selectedStartDateTime = _session.StartDateTime;
                _selectedEndDateTime = _session.EndDateTime;
                _selectedStartPoint = _session.StartPoint;
                _selectedEndPoint = _session.EndPoint;
                _selectedIntermediaries = _session.IntermediatePoints;
                _labelStartDateTimeValue.Text = "Start Date: " +  _selectedStartDateTime?.ToString(Constants.DateTimeFormat);
                _labelEndDateTimeValue.Text = "End Date: " + _selectedEndDateTime?.ToString(Constants.DateTimeFormat);
                _labelSelectedStartLocation.Text = "Start Location: " + _selectedStartPoint.X + " " + _selectedStartPoint.Y;
                _labelSelectedEndLocation.Text = "End Location: " + _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                _labelSelectedIntermediaries.Text = "Selected " + _selectedIntermediaries.Count + " Intermediate Points";
                _mediaListView.Adapter = new MediaThumbnailViewModelAdapter(this, _mediaList);
            }
        }

        //============================================================
        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        //============================================================
        private void SetupActivityFields()
        {
            _textDescription = FindViewById<TextInputEditText>(Resource.Id.sessionEditTextDescription);
            _labelStartDateTimeValue = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedStartDateTime);
            _labelEndDateTimeValue = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedEndDateTime);
            _labelSelectedStartLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedStartPosition);
            _labelSelectedEndLocation = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedEndPosition);
            _labelSelectedIntermediaries = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedIntermediaries);
            _mediaListView = FindViewById<ListView>(Resource.Id.sessionEditMediaList);
            _mediaButtonAdd = FindViewById<Button>(Resource.Id.mediasButtonAdd);
            _mediaButtonModify = FindViewById<Button>(Resource.Id.mediasButtonModify);
            _mediaButtonDelete = FindViewById<Button>(Resource.Id.mediasButtonDelete);
            _mediaButtonView = FindViewById<Button>(Resource.Id.mediasButtonView);
            _buttonSubmit = FindViewById<Button>(Resource.Id.sessionEditButtonSubmit);

            _mediaButtonAdd.Click += OnMediaButtonAddClick;
            _mediaButtonModify.Click += OnMediaButtonModifyClick;
            _mediaButtonDelete.Click += OnMediaButtonDeleteClick;
            _mediaButtonView.Click += OnMediaButtonViewClick;
            _buttonSubmit.Click += OnButtonSubmitClick;
            _labelStartDateTimeValue.Click += ButtonSelectStartDateOnClick;
            _labelEndDateTimeValue.Click += ButtonSelectEndDateOnClick;
            _labelSelectedStartLocation.Click += LabelSelectedStartLocationOnClick;
            _labelSelectedEndLocation.Click += LabelSelectedEndLocationOnClick;
            _labelSelectedIntermediaries.Click += LabelSelectedIntermediariesOnClick;
            _mediaListView.ItemClick += OnMediaListViewItemClick;
        }

        //============================================================
        private async Task RefreshDataSource(bool fromServer)
        {
            if (fromServer)
            {
                _mediaList = (await _mediaService.GetMediaAsync(_user.Id)).Where(x => x.SessionId == _session.Id).ToList();
            }
            _mediaListView.Adapter?.Dispose();
            _mediaListView.Adapter = new MediaThumbnailViewModelAdapter(this, _mediaList);
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
            await using var stream = filedata.GetStream();
            var mediaId = await _mediaService.SetMediaStreamAsync(stream, mediaType, _user.Id, string.Empty);
            var media = (await _mediaService.GetMediaAsync(_user.Id)).First(x => x.Id == mediaId);
            if (Intent.HasExtra("session"))
            {
                media.SessionId = _session.Id;
                await _mediaService.UpdateMediaAsync(media);
                await RefreshDataSource(true);
            }
            else
            {
                _mediaList.Add(media);
                await RefreshDataSource(false);
            }
            progressDialog.Dismiss();
        }

        //============================================================
        private void OnMediaButtonModifyClick(object sender, EventArgs e)
        {
            //TODO DECIDE WHETHER TO KEEP OR NOT
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
                Toast.MakeText(this, "Media deleted!", ToastLength.Short).Show();

                if (Intent.HasExtra("session"))
                {
                    await RefreshDataSource(true);
                }
                else
                {
                    _mediaList.Remove(media);
                    await RefreshDataSource(false);
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
        private void OnMediaListViewItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            _selectedMediaView?.SetBackgroundResource(0);
            e.View.SetBackgroundResource(Resource.Drawable.list_element_border);
            _selectedMediaPosition = e.Position;
            _selectedMediaView = e.View;
        }

        //============================================================
        private void LabelSelectedStartLocationOnClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(this);
            var view = LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
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
                    mapControl.Map.Layers.Add(CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(CreatePointLayer("StartPoint", Color.Green, 0.5, new Mapsui.Geometries.Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                }
            };
            alert.SetView(view);
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedStartPoint = selectedStartPoint;
                if (_selectedStartPoint != null)
                {
                    _labelSelectedStartLocation.Text = "Start Location: " + _selectedStartPoint.X + " " + _selectedStartPoint.Y;
                }
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void LabelSelectedEndLocationOnClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(this);
            var view = LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
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
                    mapControl.Map.Layers.Add(CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(CreatePointLayer("EndPoint", Color.Red, 0.5, new Mapsui.Geometries.Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                }
            };
            alert.SetView(view);
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedEndPoint = selectedEndPoint;
                if (_selectedEndPoint != null)
                {
                    _labelSelectedEndLocation.Text = "End Location: " + _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                }
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void LabelSelectedIntermediariesOnClick(object sender, EventArgs e)
        {
            var alert = new AlertDialog.Builder(this);
            var view = LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
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
                    mapControl.Map.Layers.Add(CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(CreatePointLayer("Intermediaries", Color.Yellow, 0.5, selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
                }
            };
            alert.SetView(view);
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedIntermediaries = new List<Point>(selectedIntermediaries);
                _labelSelectedIntermediaries.Text = "Selected " + _selectedIntermediaries.Count + " Intermediate Points";
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog.Show();
        }

        //============================================================
        private void ButtonSelectStartDateOnClick(object sender, EventArgs e)
        {
            var datePickerDialog = new DatePickerDialog(this, (o, args) =>
            {
                _selectedStartDateTime = args.Date;
                var timePickerDialog = new TimePickerDialog(this, (sender1, eventArgs) =>
                {
                    _selectedStartDateTime = _selectedStartDateTime?.AddHours(eventArgs.HourOfDay);
                    _selectedStartDateTime = _selectedStartDateTime?.AddMinutes(eventArgs.Minute);
                    _labelStartDateTimeValue.Text = "Start Date: " + _selectedStartDateTime?.ToString(Constants.DateTimeFormat);
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
                    _selectedEndDateTime = _selectedEndDateTime?.AddHours(eventArgs.HourOfDay);
                    _selectedEndDateTime = _selectedEndDateTime?.AddMinutes(eventArgs.Minute);
                    _labelEndDateTimeValue.Text = "End Date: " + _selectedEndDateTime?.ToString(Constants.DateTimeFormat);
                }, DateTime.Now.Hour, DateTime.Now.Minute, true);

                timePickerDialog.Show();
            }, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            datePickerDialog.Show();
        }

        //============================================================
        private async void OnButtonSubmitClick(object sender, EventArgs e)
        {
            if (_mediaList.Count == 0)
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
            }

            if(!(Intent.HasExtra("session")))
            {
                var session = new Session(_textDescription.Text.Trim(), _selectedStartDateTime.Value,
                    _selectedEndDateTime.Value,
                    _selectedStartPoint,
                    _selectedEndPoint,
                    _selectedIntermediaries, false, default, _user.Id);
                session.Id = await _sessionService.SetAsync(session);
                foreach (var media in _mediaList)
                {
                    media.SessionId = session.Id;
                    await _mediaService.UpdateMediaAsync(media);
                }
            }
            else
            {
                await _sessionService.UpdateAsync(_session);
                foreach (var media in _mediaList.Where(x => x.SessionId != _session.Id))
                {
                    media.SessionId = _session.Id;
                    await _mediaService.UpdateMediaAsync(media);
                }
            }

            Finish();
        }

        //============================================================
        private Map SetupMap()
        {
            var map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(CreatePointLayer("CurrentLocation", Color.Blue, 0.5, SphericalMercator.FromLonLat(_currentLocation.Longitude, _currentLocation.Latitude)));

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
                map.Layers.Add(CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
            }

            if (_selectedEndPoint != null)
            {
                map.Layers.Add(CreatePointLayer("EndPoint", Color.Red, 0.5, SphericalMercator.FromLonLat(_selectedEndPoint.X, _selectedEndPoint.Y)));
            }

            if (_selectedStartPoint != null)
            {
                map.Layers.Add(CreatePointLayer("StartPoint", Color.Green, 0.5, SphericalMercator.FromLonLat(_selectedStartPoint.X, _selectedStartPoint.Y)));
            }

            if (_selectedIntermediaries != null && _selectedIntermediaries.Count != 0)
            {
                map.Layers.Add(CreatePointLayer("Intermediaries", Color.Yellow, 0.5, _selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
            }

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

            return map;
        }

        //============================================================
        private static MemoryLayer CreatePointLayer(string name, Color markerColor, double markerScale, params Mapsui.Geometries.Point[] points)
        {
            return new MemoryLayer
            {
                Name = name,
                IsMapInfoLayer = true,
                DataSource = new MemoryProvider(points),
                Style = new SymbolStyle
                {
                    Fill = new Brush(markerColor),
                    SymbolScale = markerScale
                }
            };
        }

        //============================================================
        private static MemoryLayer CreateLineLayer(string name, Color lineColor, double lineWidth, params Mapsui.Geometries.Point[] points)
        {
            var feature = new Feature
            {
                Geometry = new LineString(points)
            };
            feature.Styles.Add(new VectorStyle
            {
                Line =
                {
                    Color = lineColor,
                    Width = lineWidth
                }
            });

            return new MemoryLayer
            {
                Name = name,
                IsMapInfoLayer = true,
                DataSource = new MemoryProvider(feature)
            };
        }
    }
}