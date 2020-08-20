using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using DrivingAssistant.AndroidApp.Tools;
using Mapsui;
using Mapsui.Geometries;
using Mapsui.Projection;
using Mapsui.Styles;
using Mapsui.Utilities;
using Mapsui.Widgets;
using Mapsui.Widgets.ScaleBar;
using Mapsui.Widgets.Zoom;
using Xamarin.Essentials;

namespace DrivingAssistant.AndroidApp.Activities.Map
{
    public class MapActivityViewPresenter : ViewPresenter
    {
        //============================================================
        public MapActivityViewPresenter(Context context)
        {
            _context = context;
        }

        //============================================================
        public async Task SetupMap(Point[] points)
        {
            try
            {
                var location = await Geolocation.GetLocationAsync();
                var map = new Mapsui.Map
                {
                    CRS = "EPSG:3857",
                    Transformation = new MinimalTransformation()
                };
                map.Layers.Add(OpenStreetMap.CreateTileLayer());
                map.Layers.Add(MapTools.CreatePointLayer("CurrentLocation", Color.Blue, 0.5, MapTools.ConvertFromLonLat(new Point(location.Longitude, location.Latitude)).ToArray()));
                map.Layers.Add(MapTools.CreatePointLayer("StartPoint", Color.Green, 0.5, MapTools.ConvertFromLonLat(points.First()).ToArray()));
                map.Layers.Add(MapTools.CreatePointLayer("EndPoint", Color.Red, 0.5, MapTools.ConvertFromLonLat(points.Last()).ToArray()));
                map.Layers.Add(MapTools.CreateLineLayer("Line", Color.Orange, 3, MapTools.ConvertFromLonLat(points).ToArray()));
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

                Notify(new NotificationEventArgs(NotificationCommand.MapActivity_SetupMap, map));
            }
            catch (Exception ex)
            {
                Notify(new NotificationEventArgs(NotificationCommand.MapActivity_SetupMap, ex));
            }
        }
    }
}