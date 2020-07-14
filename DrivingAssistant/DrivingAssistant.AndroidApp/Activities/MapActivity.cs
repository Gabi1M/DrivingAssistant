using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Itinero;
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
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
using Color = Mapsui.Styles.Color;
using Point = Mapsui.Geometries.Point;

namespace DrivingAssistant.AndroidApp.Activities
{
    [Activity(Label = "MapActivity")]
    public class MapActivity : Activity
    {
        private MapControl _mapControl;

        private readonly List<PointF> _points = new List<PointF>();

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_map);

            _mapControl = FindViewById<MapControl>(Resource.Id.mapControl);

            var str = Intent.GetStringExtra("points");
            var firstPoint = str.Split(' ').First();
            var secondPoint = str.Split(' ').Last();
            _points.Add(new PointF(Convert.ToSingle(firstPoint.Split(',')[0]), Convert.ToSingle(firstPoint.Split(',')[1])));
            _points.Add(new PointF(Convert.ToSingle(secondPoint.Split(',')[0]), Convert.ToSingle(secondPoint.Split(',')[1])));
            SetupMap();
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
            map.Layers.Add(CreatePointLayer());
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
        }

        //============================================================
        private MemoryLayer CreatePointLayer()
        {
            return new MemoryLayer
            {
                Name = "Points",
                IsMapInfoLayer = true,
                DataSource = new MemoryProvider(ConvertPointsToFeatures()),
                Style = new SymbolStyle
                {
                    Fill = new Brush(Color.Red),
                    SymbolScale = 0.5
                }
            };
        }

        //============================================================
        private IEnumerable<Feature> ConvertPointsToFeatures()
        {
            return _points.Select(x =>
            {
                var feature = new Feature { Geometry = SphericalMercator.FromLonLat(x.Y, x.X), ["description"] = "caca" };
                return feature;
            });
        }

        //============================================================
        private Route GetRoute()
        {
            using var stream = Assets.Open("romania.routerdb");
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var routerDb = RouterDb.Deserialize(memoryStream);
            var router = new Router(routerDb);

            var routerPoint1 = new Coordinate(_points[0].X, _points[0].Y);
            var routerPoint2 = new Coordinate(_points[1].X, _points[1].Y);

            return router.Calculate(Vehicle.Car.Fastest(), new[] {routerPoint1, routerPoint2});
        }

        //============================================================
        private ILayer GetRouteLayer(Route route)
        {
            var points = new List<Point>();
            foreach (var coordinate in route.Shape)
            {
                var spherical = SphericalMercator.FromLonLat(coordinate.Longitude, coordinate.Latitude);
                points.Add(new Point(spherical.X, spherical.Y));
            }

            var lineString = new LineString(points);
            var feature = new Feature
            {
                Geometry = lineString,
                ["Name"] = "Route 1",
                Styles = new List<IStyle> {new VectorStyle {Line = new Pen(Color.Blue, 6)}}
            };

            return new MemoryLayer
            {
                Name = "Route",
                DataSource = new MemoryProvider(feature),
                Style = null
            };
        }
    }
}