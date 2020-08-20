using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Widget;
using DrivingAssistant.AndroidApp.Tools;
using Mapsui.Projection;
using Mapsui.UI.Android;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Point = Mapsui.Geometries.Point;

namespace DrivingAssistant.AndroidApp.Activities.Map
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity
    {
        private MapControl _mapControl;
        private Point[] _sessionPoints;

        private MapActivityViewPresenter _viewPresenter;
        
        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_map);
            SetupActivityFields();
            GetPointsFromIntent();
            _viewPresenter = new MapActivityViewPresenter(this);
            _viewPresenter.OnNotificationReceived += ViewPresenterOnNotificationReceived;
            SetupMap();
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
                case NotificationCommand.MapActivity_SetupMap:
                {
                    _mapControl.Map = e.Data as Mapsui.Map;
                    var middlePoint = new Point((_sessionPoints.First().X + _sessionPoints.Last().X) / 2, (_sessionPoints.First().Y + _sessionPoints.Last().Y) / 2);
                    _mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(middlePoint.X, middlePoint.Y), _mapControl.Map.Resolutions[9]);
                    break;
                }
            }
        }

        //============================================================
        private void SetupActivityFields()
        {
            _mapControl = FindViewById<MapControl>(Resource.Id.mapControl);
        }

        //============================================================
        private async void SetupMap()
        {
            await _viewPresenter.SetupMap(_sessionPoints);
        }

        //============================================================
        private void GetPointsFromIntent()
        {
            var pointList = new List<Point>();

            var startLocationLonLat = JsonConvert.DeserializeObject<Core.Models.Point>(Intent?.GetStringExtra("startPoint")!);
            var endLocationLonLat = JsonConvert.DeserializeObject<Core.Models.Point>(Intent?.GetStringExtra("endPoint")!);
            var waypoints = JsonConvert.DeserializeObject<ICollection<Core.Models.Point>>(Intent?.GetStringExtra("waypoints")!);

            var startPoint = new Point(startLocationLonLat.X, startLocationLonLat.Y);
            var endPoint = new Point(endLocationLonLat.X, endLocationLonLat.Y);

            pointList.Add(startPoint);
            pointList.AddRange(waypoints.Select(waypoint => new Point(waypoint.X, waypoint.Y)));
            pointList.Add(endPoint);

            _sessionPoints =  pointList.ToArray();
        }
    }
}