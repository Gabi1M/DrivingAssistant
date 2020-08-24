using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using DrivingAssistant.AndroidApp.Services;
using DrivingAssistant.AndroidApp.Tools;
using DrivingAssistant.Core.Enums;
using DrivingAssistant.Core.Models;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Styles;
using Mapsui.UI.Android;
using Mapsui.Utilities;
using Mapsui.Widgets.Zoom;
using Plugin.FilePicker;
using Location = Xamarin.Essentials.Location;

namespace DrivingAssistant.AndroidApp.Activities.SessionEdit
{
    public class SessionEditActivityViewPresenter : ViewPresenter
    {
        private readonly SessionService _sessionService = new SessionService();
        private readonly VideoService _videoService = new VideoService();
        private readonly User _user;
        private readonly Location _currentLocation;

        private ICollection<VideoRecording> _videos = new List<VideoRecording>();
        private int _selectedPosition = -1;
        private View _selectedView;

        private DrivingSession _currentDrivingSession;
        private DateTime? _selectedStartDateTime;
        private DateTime? _selectedEndDateTime;
        private LocationPoint _selectedStartLocationPoint;
        private LocationPoint _selectedEndLocationPoint;
        private ICollection<LocationPoint> _selectedWaypoints = new List<LocationPoint>();

        //============================================================
        public SessionEditActivityViewPresenter(Context context, DrivingSession currentDrivingSession, User user, Location currentLocation)
        {
            _context = context;
            _currentDrivingSession = currentDrivingSession;
            _user = user;
            _currentLocation = currentLocation;

            if (_currentDrivingSession != null)
            {
                _selectedStartLocationPoint = currentDrivingSession.StartLocation;
                _selectedEndLocationPoint = currentDrivingSession.EndLocation;
                _selectedWaypoints = currentDrivingSession.Waypoints;
            }
        }

        //============================================================
        public async Task BackPressed()
        {
            foreach (var video in _videos.Where(x => !x.IsInSession()))
            {
                try
                {
                    await _videoService.DeleteVideoAsync(video.Id);
                }
                catch (Exception ex)
                {
                    Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_Back, ex));
                }
            }
            Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_Back, null));
        }

        //============================================================
        public void ItemClick(int position, View view)
        {
            _selectedView?.SetBackgroundResource(0);
            _selectedPosition = position;
            _selectedView = view;
            Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_ItemClick, view));
        }

        //============================================================
        public async Task VideoAddClick()
        {
            var filedata = await CrossFilePicker.Current.PickFile();
            if (filedata == null)
            {
                return;
            }

            if (Path.GetExtension(filedata.FilePath) != ".jpg" && Path.GetExtension(filedata.FilePath) != ".mp4")
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_VideoRefresh, new Exception("Selected file is not mp4 video file")));
                return;
            }

            var alert = new AlertDialog.Builder(_context);
            alert.SetTitle("Choose a unique description for this media");
            var textEdit = new EditText(_context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent),
                Gravity = GravityFlags.Center
            };
            alert.SetView(textEdit);
            alert.SetPositiveButton("Ok", async (o, args) =>
            {
                var progressDialog = ProgressDialog.Show(_context, "Video Upload", "Uploading...");
                try
                {
                    var videoId = await _videoService.SetVideoStreamAsync(filedata.GetStream(), textEdit.Text);
                    var video = await _videoService.GetVideoByIdAsync(videoId);
                    if (_currentDrivingSession == null)
                    {
                        _videos.Add(video);
                        await RefreshVideoSource();
                    }
                    else
                    {
                        video.SessionId = _currentDrivingSession.Id;
                        await _videoService.UpdateVideoAsync(video);
                        await RefreshVideoSource(true);
                    }

                    Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_VideoRefresh, _videos));
                }
                catch (Exception ex)
                {
                    Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_VideoRefresh, ex));
                }
                finally
                {
                    progressDialog?.Dismiss();
                }
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create()?.Show();
        }

        //============================================================
        public void VideoDeleteClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_VideoRefresh, new Exception("No video selected!")));
                return;
            }

            var alert = new AlertDialog.Builder(_context);
            alert.SetTitle("Confirm Delete");
            alert.SetMessage("Action cannot be undone");
            alert.SetPositiveButton("Delete", async (o, args) =>
            {
                var video = _videos.ElementAt(_selectedPosition);
                try
                {
                    await _videoService.DeleteVideoAsync(video.Id);
                }
                catch (Exception ex)
                {
                    Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_VideoRefresh, ex));
                    return;
                }

                if (_currentDrivingSession == null)
                {
                    _videos.Remove(video);
                    await RefreshVideoSource();
                }
                else
                {
                    await RefreshVideoSource(true);
                }
            });

            alert.SetNegativeButton("Cancel", (o, args) => { });
            var dialog = alert.Create();
            dialog?.Show();
        }

        //============================================================
        private async Task RefreshVideoSource(bool fetchRemote = false)
        {
            if (fetchRemote)
            {
                try
                {
                    _videos = (await _videoService.GetVideoBySessionAsync(_currentDrivingSession.Id)).ToList();
                }
                catch (Exception ex)
                {
                    Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_VideoRefresh, ex));
                }
            }

            Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_VideoRefresh, _videos));
        }

        //============================================================
        public void VideoViewClick()
        {
            if (_selectedPosition == -1)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_VideoRefresh, new Exception("No video selected!")));
                return;
            }

            Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_VideoView, _videos.ElementAt(_selectedPosition)));
        }

        //============================================================
        public void StartDateClick()
        {
            new DatePickerDialog(_context, (o, args) =>
            {
                _selectedStartDateTime = args.Date;
                new TimePickerDialog(_context, (sender1, eventArgs) =>
                {
                    _selectedStartDateTime = _selectedStartDateTime?.AddHours(eventArgs.HourOfDay);
                    _selectedStartDateTime = _selectedStartDateTime?.AddMinutes(eventArgs.Minute);
                    Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_StartDate, _selectedStartDateTime));
                }, DateTime.Now.Hour, DateTime.Now.Minute, true).Show();
            }, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Show();
        }

        //============================================================
        public void EndDateClick()
        {
            new DatePickerDialog(_context, (o, args) =>
            {
                _selectedEndDateTime = args.Date; new TimePickerDialog(_context, (sender1, eventArgs) =>
                {
                    _selectedEndDateTime = _selectedEndDateTime?.AddHours(eventArgs.HourOfDay);
                    _selectedEndDateTime = _selectedEndDateTime?.AddMinutes(eventArgs.Minute);
                    Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_EndDate, _selectedEndDateTime));
                }, DateTime.Now.Hour, DateTime.Now.Minute, true).Show();
            }, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).Show();
        }

        //============================================================
        public void StartLocationClick()
        {
            var alert = new AlertDialog.Builder(_context);
            var view = (_context as Activity)?.LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            alert.SetView(view);
            var mapControl = view?.FindViewById<MapControl>(Resource.Id.mapControl);
            mapControl.Map = SetupMap();
            mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(_currentLocation.Longitude, _currentLocation.Latitude), mapControl.Map.Resolutions[9]);
            var selectedStartPoint = _selectedStartLocationPoint;
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
                    selectedStartPoint = new LocationPoint(tempPoint.X, tempPoint.Y);

                    var points = new List<Point>
                    {
                        new Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)
                    };
                    points.AddRange(_selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
                    if (_selectedEndLocationPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedEndLocationPoint.X, _selectedEndLocationPoint.Y));
                    }
                    mapControl.Map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(MapTools.CreatePointLayer("StartPoint", Color.Green, 0.5, new Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                }
            };
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedStartLocationPoint = selectedStartPoint;
                if (_selectedStartLocationPoint != null)
                {
                    Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_StartLocation, _selectedStartLocationPoint));
                }
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create()?.Show();
        }

        //============================================================
        public void EndLocationClick()
        {
            var alert = new AlertDialog.Builder(_context);
            var view = (_context as Activity)?.LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view?.FindViewById<MapControl>(Resource.Id.mapControl);
            alert.SetView(view);
            mapControl.Map = SetupMap();
            mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(_currentLocation.Longitude, _currentLocation.Latitude), mapControl.Map.Resolutions[9]);
            var selectedEndPoint = _selectedEndLocationPoint;
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
                    selectedEndPoint = new LocationPoint(tempPoint.X, tempPoint.Y);

                    var points = new List<Point>();
                    if (_selectedStartLocationPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedStartLocationPoint.X, _selectedStartLocationPoint.Y));
                    }
                    points.AddRange(_selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
                    points.Add(new Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y));
                    mapControl.Map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(MapTools.CreatePointLayer("EndPoint", Color.Red, 0.5, new Point(args.MapInfo.WorldPosition.X, args.MapInfo.WorldPosition.Y)));
                }
            };
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedEndLocationPoint = selectedEndPoint;
                if (_selectedEndLocationPoint != null)
                {
                    Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_EndLocation, _selectedEndLocationPoint));
                }
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create()?.Show();
        }

        //============================================================
        public void WaypointsClick()
        {
            var alert = new AlertDialog.Builder(_context);
            var view = (_context as Activity)?.LayoutInflater.Inflate(Resource.Layout.activity_map, null);
            var mapControl = view?.FindViewById<MapControl>(Resource.Id.mapControl);
            alert.SetView(view);
            mapControl.Map = SetupMap();
            mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(_currentLocation.Longitude, _currentLocation.Latitude), mapControl.Map.Resolutions[9]);
            var selectedWaypoints = new List<LocationPoint>(_selectedWaypoints!);
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
                        selectedWaypoints.Remove(selectedWaypoints.First(x => featurePoint.Equals(SphericalMercator.FromLonLat(x.X, x.Y))));
                    }
                    else
                    {
                        selectedWaypoints.Add(new LocationPoint(tempPoint.X, tempPoint.Y));
                    }

                    var points = new List<Point>();
                    if (_selectedStartLocationPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedStartLocationPoint.X, _selectedStartLocationPoint.Y));
                    }
                    points.AddRange(selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
                    if (_selectedEndLocationPoint != null)
                    {
                        points.Add(SphericalMercator.FromLonLat(_selectedEndLocationPoint.X, _selectedEndLocationPoint.Y));
                    }
                    mapControl.Map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
                    mapControl.Map.Layers.Add(MapTools.CreatePointLayer("Waypoints", Color.Yellow, 0.5, selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
                }
            };
            alert.SetPositiveButton("Confirm", (o, args) =>
            {
                _selectedWaypoints = new List<LocationPoint>(selectedWaypoints);
                Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_Waypoints, _selectedWaypoints));
            });
            alert.SetNegativeButton("Cancel", (o, args) => { });
            alert.Create()?.Show();
        }

        //============================================================
        public async Task SubmitClick(string name)
        {
            try
            {
                if (_currentDrivingSession != null)
                {
                    _currentDrivingSession.Name = name;
                    _currentDrivingSession.StartDateTime = _selectedStartDateTime ?? DateTime.Now;
                    _currentDrivingSession.EndDateTime = _selectedEndDateTime ?? DateTime.Now;
                    _currentDrivingSession.StartLocation = _selectedStartLocationPoint;
                    _currentDrivingSession.EndLocation = _selectedEndLocationPoint;
                    _currentDrivingSession.Waypoints = _selectedWaypoints;
                    _currentDrivingSession.Id = await _sessionService.SetAsync(_currentDrivingSession);
                    foreach (var video in _videos.Where(x => x.SessionId != _currentDrivingSession.Id))
                    {
                        video.SessionId = _currentDrivingSession.Id;
                        await _videoService.UpdateVideoAsync(video);
                    }
                }
                else
                {
                    _currentDrivingSession = new DrivingSession
                    {
                        Name = name,
                        StartDateTime = _selectedStartDateTime ?? DateTime.Now,
                        EndDateTime = _selectedEndDateTime ?? DateTime.Now,
                        StartLocation = _selectedStartLocationPoint ?? new LocationPoint(0,0),
                        EndLocation = _selectedEndLocationPoint ?? new LocationPoint(0,0),
                        Waypoints = _selectedWaypoints,
                        Id = -1,
                        Status = SessionStatus.Unprocessed,
                        UserId = _user.Id,
                        DateAdded = DateTime.Now
                    };
                    _currentDrivingSession.Id = await _sessionService.SetAsync(_currentDrivingSession);
                    foreach (var video in _videos)
                    {
                        video.SessionId = _currentDrivingSession.Id;
                        await _videoService.UpdateVideoAsync(video);
                    }
                }
                Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_Submit, null));
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.SessionEditActivity_Submit, ex));
            }
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

            if (_selectedStartLocationPoint != null || _selectedEndLocationPoint != null || (_selectedWaypoints != null && _selectedWaypoints.Count != 0))
            {
                var points = new List<Point>();
                if (_selectedStartLocationPoint != null)
                {
                    points.Add(SphericalMercator.FromLonLat(_selectedStartLocationPoint.X, _selectedStartLocationPoint.Y));
                }

                if (_selectedWaypoints != null && _selectedWaypoints.Count != 0)
                {
                    points.AddRange(_selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)));
                }

                if (_selectedEndLocationPoint != null)
                {
                    points.Add(SphericalMercator.FromLonLat(_selectedEndLocationPoint.X, _selectedEndLocationPoint.Y));
                }
                map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 5, points.ToArray()));
            }

            if (_selectedEndLocationPoint != null)
            {
                map.Layers.Add(MapTools.CreatePointLayer("EndPoint", Color.Red, 0.5, SphericalMercator.FromLonLat(_selectedEndLocationPoint.X, _selectedEndLocationPoint.Y)));
            }

            if (_selectedStartLocationPoint != null)
            {
                map.Layers.Add(MapTools.CreatePointLayer("StartPoint", Color.Green, 0.5, SphericalMercator.FromLonLat(_selectedStartLocationPoint.X, _selectedStartLocationPoint.Y)));
            }

            if (_selectedWaypoints != null && _selectedWaypoints.Count != 0)
            {
                map.Layers.Add(MapTools.CreatePointLayer("Waypoints", Color.Yellow, 0.5, _selectedWaypoints.Select(x => SphericalMercator.FromLonLat(x.X, x.Y)).ToArray()));
            }

            return map;
        }
    }
}