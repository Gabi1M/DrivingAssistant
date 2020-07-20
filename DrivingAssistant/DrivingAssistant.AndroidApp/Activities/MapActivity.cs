﻿using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
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
        private async void SetupMap()
        {
            var location = await Geolocation.GetLocationAsync();
            var map = new Map
            {
                CRS = "EPSG:3857",
                Transformation = new MinimalTransformation()
            };
            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            map.Layers.Add(CreatePointLayer("CurrentLocation", Color.Blue, 0.5, ConvertFromLonLat(new Point(location.Longitude, location.Latitude))));
            map.Layers.Add(CreatePointLayer("StartPoint", Color.Green, 0.5, ConvertFromLonLat(_sessionPoints.First())));
            map.Layers.Add(CreatePointLayer("EndPoint", Color.Red, 0.5, ConvertFromLonLat(_sessionPoints.Last())));
            map.Layers.Add(CreateLineLayer("Line", Color.Orange, 3, ConvertFromLonLat(_sessionPoints)));
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
        private static MemoryLayer CreateLineLayer(string name, Color lineColor, double lineWidth, params Point[] points)
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

        //============================================================
        private Point[] GetPointsFromIntent()
        {
            var pointList = new List<Mapsui.Geometries.Point>();

            var startPointLonLat = JsonConvert.DeserializeObject<Core.Models.Point>(Intent.GetStringExtra("startPoint"));
            var endPointLonLat = JsonConvert.DeserializeObject<Core.Models.Point>(Intent.GetStringExtra("endPoint"));
            var intermediatePointsLonLat = JsonConvert.DeserializeObject<ICollection<Core.Models.Point>>(Intent.GetStringExtra("intermediatePoints"));

            var startPoint = new Mapsui.Geometries.Point(startPointLonLat.X, startPointLonLat.Y);
            var endPoint = new Mapsui.Geometries.Point(endPointLonLat.X, endPointLonLat.Y);

            pointList.Add(startPoint);
            pointList.AddRange(intermediatePointsLonLat.Select(intermediatePoint => new Mapsui.Geometries.Point(intermediatePoint.X, intermediatePoint.Y)));
            pointList.Add(endPoint);

            return pointList.ToArray();
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