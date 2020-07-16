using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Itinero;
using Itinero.LocalGeo;
using Mapsui;
using Mapsui.Geometries;
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
        private LinearLayout _popup;
        private TextView _popupTextView;
        private Button _popupButton;

        //============================================================
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_map);

            _mapControl = FindViewById<MapControl>(Resource.Id.mapControl);
            _popup = CreatePopup();

            FindViewById<RelativeLayout>(Resource.Id.mapLayout).AddView(_popup);

            SetupMap();
        }

        //============================================================
        private LinearLayout CreatePopup()
        {
            var linearLayout = new LinearLayout(this);
            linearLayout.AddView(CreateTextView());
            linearLayout.AddView(CreateButton());
            linearLayout.SetPadding(5, 5, 5, 5);
            linearLayout.SetBackgroundColor(Android.Graphics.Color.Transparent);
            linearLayout.Visibility = ViewStates.Gone;
            return linearLayout;
        }

        //============================================================
        private TextView CreateTextView()
        {
            _popupTextView = new TextView(this)
            {
                TextSize = 16,
                Text = "Native Android",
                LayoutParameters = new RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
            };
            _popupTextView.SetPadding(3, 3, 3, 3);
            return _popupTextView;
        }

        //============================================================
        private Button CreateButton()
        {
            _popupButton = new Button(this)
            {
                TextSize = 6,
                Text = "Button",
                LayoutParameters = new RelativeLayout.LayoutParams(
                    ViewGroup.LayoutParams.WrapContent,
                    ViewGroup.LayoutParams.WrapContent)
            };
            _popupButton.SetPadding(3,3,3,3);
            return _popupButton;
        }

        //============================================================
        private Point[] GetConvertedPointsFromIntent()
        {
            var startPoint = JsonConvert.DeserializeObject<Coordinate>(Intent.GetStringExtra("startPoint"));
            var endPoint = JsonConvert.DeserializeObject<Coordinate>(Intent.GetStringExtra("endPoint"));

            var startPointConverted = SphericalMercator.FromLonLat(startPoint.Latitude, startPoint.Longitude);
            var endPointConverted = SphericalMercator.FromLonLat(endPoint.Latitude, endPoint.Longitude);

            return new[] { startPointConverted, endPointConverted };
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
            map.Layers.Add(CreatePointLayer(GetConvertedPointsFromIntent()));
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
                    if (_popup != null && _popup.Visibility != ViewStates.Gone)
                    {
                        _popup.Visibility = ViewStates.Gone;
                    }

                    var zoomWidgetEnvelope = _mapControl.Map.Widgets.First(x => x.GetType() == typeof(ZoomInOutWidget)).Envelope;
                    if (!zoomWidgetEnvelope.Contains(e.MapInfo.ScreenPosition))
                    {
                        _mapControl.Map.Layers.Remove(_mapControl.Map.Layers.FindLayer("Points").First());
                        _mapControl.Map.Layers.Add(CreatePointLayer(new Point(e.MapInfo.WorldPosition.X, e.MapInfo.WorldPosition.Y)));
                    }
                }
                else
                {
                    var screenPosition = _mapControl.Viewport.WorldToScreen(e.MapInfo.Feature.Geometry.BoundingBox.Centroid);
                    var screenPositionInPixels = _mapControl.ToPixels(screenPosition);

                    _popup.SetX((float)screenPositionInPixels.X);
                    _popup.SetY((float)screenPositionInPixels.Y);

                    _popup.Visibility = ViewStates.Visible;
                    _popupTextView.Text = SphericalMercator.ToLonLat(e.MapInfo.WorldPosition.X, e.MapInfo.WorldPosition.Y).ToString();
                }
            }
        }

        //============================================================
        private static MemoryLayer CreatePointLayer(params Point[] points)
        {
            return new MemoryLayer
            {
                Name = "Points",
                IsMapInfoLayer = true,
                DataSource = new MemoryProvider(points),
                Style = new SymbolStyle
                {
                    Fill = new Brush(Color.Red),
                    SymbolScale = 0.5
                }
            };
        }

        //============================================================
        private Route GetRoute()
        {
            using var stream = Assets.Open("romania.routerdb");
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var routerDb = RouterDb.Deserialize(memoryStream);
            var router = new Router(routerDb);
            return null;
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