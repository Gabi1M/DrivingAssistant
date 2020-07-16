using System.Linq;
using Android.App;
using Android.OS;
using Itinero.LocalGeo;
using Mapsui;
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
using Color = Mapsui.Styles.Color;
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
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
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
        private void SetupMap()
        {
            var map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(CreatePointLayer("SessionPoints", Color.Green, 0.5, ConvertFromLonLat(_sessionPoints)));
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
            _mapControl.Map = map;
            map.Home = n => n.NavigateTo(SphericalMercator.FromLonLat(23.598892, 46.765887), map.Resolutions[9]);
            _mapControl.Info += MapControlOnInfo;
        }

        //============================================================
        private void MapControlOnInfo(object sender, MapInfoEventArgs e)
        {
            if (e.NumTaps == 1)
            {
                if (e.MapInfo.Feature == null)
                {
                    var zoomWidgetEnvelope = _mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                    if (!zoomWidgetEnvelope.Contains(e.MapInfo.ScreenPosition))
                    {
                        if (_mapControl.Map.Layers.Any(x => x.Name == "PlacedPoint"))
                        {
                            _mapControl.Map.Layers.Remove(_mapControl.Map.Layers.FindLayer("PlacedPoint").First());
                        }
                        _mapControl.Map.Layers.Add(CreatePointLayer("PlacedPoint", Color.Red, 0.5, new Point(e.MapInfo.WorldPosition.X, e.MapInfo.WorldPosition.Y)));
                    }
                }
            }
        }

        //============================================================
        private static MemoryLayer CreatePointLayer(string name, Color markerColor, double markerScale, params Point[] points)
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
        private Point[] GetPointsFromIntent()
        {
            var startPointLonLat = JsonConvert.DeserializeObject<Coordinate>(Intent.GetStringExtra("startPoint"));
            var endPointLonLat = JsonConvert.DeserializeObject<Coordinate>(Intent.GetStringExtra("endPoint"));

            var startPoint = new Point(startPointLonLat.Latitude, startPointLonLat.Longitude);
            var endPoint = new Point(endPointLonLat.Latitude, endPointLonLat.Longitude);

            return new[] { startPoint, endPoint };
        }

        //============================================================
        private static Point[] ConvertToLonLat(params Point[] points)
        {
            return points.Select(point => SphericalMercator.ToLonLat(point.X, point.Y)).ToArray();
        }

        //============================================================
        private static Point[] ConvertFromLonLat(params Point[] points)
        {
            return points.Select(point => SphericalMercator.FromLonLat(point.X, point.Y)).ToArray();
        }
    }
}