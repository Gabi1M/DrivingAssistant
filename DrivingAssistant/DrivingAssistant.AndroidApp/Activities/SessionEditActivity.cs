using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.Core.Models;
using Mapsui;
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
using Constants = DrivingAssistant.AndroidApp.Tools.Constants;
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
        private TextView _labelSelectedMedia;
        private Button _buttonSubmit;

        private User _user;
        private Session _session;
        private SessionService _sessionService;
        private MediaService _mediaService;

        private List<Media> _selectedMedia = new List<Media>();
        private DateTime? _selectedStartDateTime;
        private DateTime? _selectedEndDateTime;
        private Point _selectedStartPoint;
        private Point _selectedEndPoint;
        private ICollection<Point> _selectedIntermediaries = new List<Point>();

        //============================================================
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_session_edit);
            SetupActivityFields();

            _sessionService = new SessionService(Constants.ServerUri);
            _mediaService = new MediaService(Constants.ServerUri);
            _user = JsonConvert.DeserializeObject<User>(Intent.GetStringExtra("user"));

            if (Intent.HasExtra("session"))
            {
                _session = JsonConvert.DeserializeObject<Session>(Intent.GetStringExtra("session"));
                _selectedMedia = (await _mediaService.GetMediaAsync(_user.Id)).Where(x => x.SessionId == _session.Id).ToList();
                _textDescription.Text = _session.Description;
                _selectedStartDateTime = _session.StartDateTime;
                _selectedEndDateTime = _session.EndDateTime;
                _selectedStartPoint = _session.StartPoint;
                _selectedEndPoint = _session.EndPoint;
                _selectedIntermediaries = _session.IntermediatePoints;
                _labelStartDateTimeValue.Text = _selectedStartDateTime?.ToString(Constants.DateTimeFormat);
                _labelEndDateTimeValue.Text = _selectedEndDateTime?.ToString(Constants.DateTimeFormat);
                _labelSelectedStartLocation.Text = _selectedStartPoint.X + " " + _selectedStartPoint.Y;
                _labelSelectedEndLocation.Text = _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                _labelEndDateTimeValue.Text = _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                if (_selectedMedia.Count == 0)
                {
                    _labelSelectedMedia.Text = "Tap to Select Media";
                }
                else
                {
                    _labelSelectedMedia.Text = "Selected " + _selectedMedia.Count + " items!";
                }
            }
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
            _labelSelectedMedia = FindViewById<TextView>(Resource.Id.sessionEditLabelSelectedMedia);
            _buttonSubmit = FindViewById<Button>(Resource.Id.sessionEditButtonSubmit);

            _labelSelectedMedia.Click += OnLabelSelectedMediaClick;
            _buttonSubmit.Click += OnButtonSubmitClick;
            _labelStartDateTimeValue.Click += ButtonSelectStartDateOnClick;
            _labelEndDateTimeValue.Click += ButtonSelectEndDateOnClick;
            _labelSelectedStartLocation.Click += LabelSelectedStartLocationOnClick;
            _labelSelectedEndLocation.Click += LabelSelectedEndLocationOnClick;
            _labelSelectedIntermediaries.Click += LabelSelectedIntermediariesOnClick;
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
            mapControl.Map = map;
            map.Home = n => n.NavigateTo(SphericalMercator.FromLonLat(23.598892, 46.765887), map.Resolutions[9]);
            mapControl.Info += (o, args) =>
            {
                var zoomWidgetEnvelope = mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                if (!zoomWidgetEnvelope.Contains(args.MapInfo.ScreenPosition))
                {
                    if (mapControl.Map.Layers.Any(x => x.Name == "StartPoint"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.FindLayer("StartPoint").First());
                    }
                    mapControl.Map.Layers.Add(CreatePointLayer("StartPoint", Color.Green, 0.5, new Mapsui.Geometries.Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                    var tempPoint = SphericalMercator.ToLonLat(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y);
                    _selectedStartPoint = new Point(tempPoint.X, tempPoint.Y);
                    _labelSelectedStartLocation.Text = _selectedStartPoint.X + " " + _selectedStartPoint.Y;
                }
            };
            alert.SetView(view);
            alert.SetPositiveButton("Confirm", (o, args) => { });
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
            var map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            map.Layers.Add(OpenStreetMap.CreateTileLayer());

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
            mapControl.Map = map;
            map.Home = n => n.NavigateTo(SphericalMercator.FromLonLat(23.598892, 46.765887), map.Resolutions[9]);
            mapControl.Info += (o, args) =>
            {
                var zoomWidgetEnvelope = mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                if (!zoomWidgetEnvelope.Contains(args.MapInfo.ScreenPosition))
                {
                    if (mapControl.Map.Layers.Any(x => x.Name == "EndPoint"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.FindLayer("EndPoint").First());
                    }
                    mapControl.Map.Layers.Add(CreatePointLayer("EndPoint", Color.Red, 0.5, new Mapsui.Geometries.Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                    var tempPoint = SphericalMercator.ToLonLat(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y);
                    _selectedEndPoint = new Point(tempPoint.X, tempPoint.Y);
                    _labelSelectedEndLocation.Text = _selectedEndPoint.X + " " + _selectedEndPoint.Y;
                }
            };
            alert.SetView(view);
            alert.SetPositiveButton("Confirm", (o, args) => { });
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
            var map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            map.Layers.Add(OpenStreetMap.CreateTileLayer());

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
            mapControl.Map = map;
            map.Home = n => n.NavigateTo(SphericalMercator.FromLonLat(23.598892, 46.765887), map.Resolutions[9]);
            mapControl.Info += (o, args) =>
            {
                var zoomWidgetEnvelope = mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                if (!zoomWidgetEnvelope.Contains(args.MapInfo.ScreenPosition))
                {
                    if (mapControl.Map.Layers.Any(x => x.Name == "Intermediaries"))
                    {
                        mapControl.Map.Layers.Remove(mapControl.Map.Layers.FindLayer("Intermediaries").First());
                    }

                    var tempPoint = SphericalMercator.ToLonLat(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y);

                    if (args.MapInfo.Feature != null && args.MapInfo.Layer.Name == "Intermediaries")
                    {
                        var featurePoint = args.MapInfo.Feature.Geometry.AllVertices().First();
                        var point = _selectedIntermediaries.First(x => featurePoint.Equals(SphericalMercator.FromLonLat(x.X, x.Y)));
                        _selectedIntermediaries.Remove(point);
                        map.Layers.Add(CreatePointLayer("Intermediaries", Color.Yellow, 0.5, _selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
                    }
                    else
                    {
                        _selectedIntermediaries.Add(new Point(tempPoint.X, tempPoint.Y));
                        map.Layers.Add(CreatePointLayer("Intermediaries", Color.Yellow, 0.5, _selectedIntermediaries.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
                    }
                }
            };
            alert.SetView(view);
            alert.SetPositiveButton("Confirm", (o, args) => { });
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
                    _labelStartDateTimeValue.Text = _selectedStartDateTime?.ToString(Constants.DateTimeFormat);
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
                    _labelEndDateTimeValue.Text = _selectedEndDateTime?.ToString(Constants.DateTimeFormat);
                }, DateTime.Now.Hour, DateTime.Now.Minute, true);

                timePickerDialog.Show();
            }, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            datePickerDialog.Show();
        }

        //============================================================
        private async void OnLabelSelectedMediaClick(object sender, EventArgs e)
        {
            var medias = (await _mediaService.GetMediaAsync(_user.Id)).Where(x => x.IsProcessed() == false).OrderBy(x => x.DateAdded).ToList();
            var mediasString = medias.Select(x => x.Type + ", " + x.Description).ToArray();

            var checkedItems = medias.Select(media => _selectedMedia.Any(x => x.Id == media.Id)).ToArray();
            var alert = new AlertDialog.Builder(this);
            alert.SetTitle("Choose media to include in this session");
            alert.SetMultiChoiceItems(mediasString, checkedItems, (o, args) =>
            {
                if (args.IsChecked == false && checkedItems[args.Which])
                {
                    checkedItems[args.Which] = false;
                }

                if (args.IsChecked && !checkedItems[args.Which])
                {
                    checkedItems[args.Which] = true;
                }
            });

            alert.SetPositiveButton("Ok", (o, args) =>
            {
                _selectedMedia.Clear();
                for (var i = 0; i < checkedItems.Length; i++)
                {
                    if (checkedItems[i])
                    {
                        _selectedMedia.Add(medias.ElementAt(i));
                    }
                }
                if (_selectedMedia.Count == 0)
                {
                    _labelSelectedMedia.Text = "Tap to Select Media";
                }
                else
                {
                    _labelSelectedMedia.Text = "Selected " + _selectedMedia.Count + " items!";
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
                _selectedEndPoint == null || _selectedIntermediaries.Count == 0)
            {
                Toast.MakeText(this, "Some fields were left blank!", ToastLength.Short).Show();
                return;
            }

            var session = new Session(_textDescription.Text.Trim(), _selectedStartDateTime.Value,
                _selectedEndDateTime.Value,
                _selectedStartPoint,
                _selectedEndPoint,
                _selectedIntermediaries, _user.Id);
            session.Id = await _sessionService.SetAsync(session);
            foreach (var media in _selectedMedia)
            {
                media.SessionId = session.Id;
                await _mediaService.UpdateMediaAsync(media);
            }

            Finish();
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
    }
}