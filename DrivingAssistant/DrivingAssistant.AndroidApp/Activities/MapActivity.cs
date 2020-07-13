using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
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
                    SymbolScale = 0.50
                }
            };
        }

        //============================================================
        private IEnumerable<Feature> ConvertPointsToFeatures()
        {
            return _points.Select(x =>
            {
                var feature = new Feature {Geometry = SphericalMercator.FromLonLat(x.Y, x.X), ["description"] = "caca"};
                return feature;
            });
        }
    }
}