using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using DrivingAssistant.AndroidApp.Tools;
using Mapsui;
using Mapsui.Projection;
using Mapsui.UI.Android;
using Mapsui.Utilities;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Color = Mapsui.Styles.Color;
using Map = Mapsui.Map;
using Point = Mapsui.Geometries.Point;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity
    {
        private MapControl _mapControl;
        private Point[] _sessionPoints;
        
        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_map);
            SetupActivityFields();
            _sessionPoints = GetPointsFromIntent();
            SetupMap();
        }

        //============================================================
        private void SetupActivityFields()
        {
            _mapControl = FindViewById<MapControl>(Resource.Id.mapControl);
        }

        //============================================================
        private async void SetupMap()
        {
            var location = await Geolocation.GetLocationAsync();
            var map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(MapTools.CreatePointLayer("CurrentLocation", Color.Blue, 0.5, MapTools.ConvertFromLonLat(new Point(location.Longitude, location.Latitude)).ToArray()));
            map.Layers.Add(MapTools.CreatePointLayer("StartPoint", Color.Green, 0.5, MapTools.ConvertFromLonLat(_sessionPoints.First()).ToArray()));
            map.Layers.Add(MapTools.CreatePointLayer("EndPoint", Color.Red, 0.5, MapTools.ConvertFromLonLat(_sessionPoints.Last()).ToArray()));
            map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 3, MapTools.ConvertFromLonLat(_sessionPoints).ToArray()));
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

            var middlePoint = new Point((_sessionPoints.First().X + _sessionPoints.Last().X) / 2, (_sessionPoints.First().Y + _sessionPoints.Last().Y) / 2);

            _mapControl.Map = map;
            _mapControl.Navigator.NavigateTo(SphericalMercator.FromLonLat(middlePoint.X, middlePoint.Y), map.Resolutions[9]);
        }

        //============================================================
        private Point[] GetPointsFromIntent()
        {
            var pointList = new List<Point>();

            var startLocationLonLat = JsonConvert.DeserializeObject<Core.Models.Point>(Intent.GetStringExtra("startPoint"));
            var endLocationLonLat = JsonConvert.DeserializeObject<Core.Models.Point>(Intent.GetStringExtra("endPoint"));
            var waypoints = JsonConvert.DeserializeObject<ICollection<Core.Models.Point>>(Intent.GetStringExtra("waypoints"));

            var startPoint = new Point(startLocationLonLat.X, startLocationLonLat.Y);
            var endPoint = new Point(endLocationLonLat.X, endLocationLonLat.Y);

            pointList.Add(startPoint);
            pointList.AddRange(waypoints.Select(waypoint => new Point(waypoint.X, waypoint.Y)));
            pointList.Add(endPoint);

            return pointList.ToArray();
        }
    }
}