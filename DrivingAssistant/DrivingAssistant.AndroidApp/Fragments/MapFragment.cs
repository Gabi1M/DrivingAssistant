using System.Collections.Generic;
using System.Linq;
using Android.OS;
using Android.Views;
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
using Fragment = Android.Support.V4.App.Fragment;

namespace DrivingAssistant.AndroidApp.Fragments
{
    public class MapFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_map, container, false);
            var mapControl = view.FindViewById<MapControl>(Resource.Id.mapControl);
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
            mapControl.Map = map;
            map.Home = n => n.NavigateTo(SphericalMercator.FromLonLat(23.598892, 46.765887), map.Resolutions[9]);
            return view;
        }

        private static MemoryLayer CreatePointLayer()
        {
            return new MemoryLayer
            {
                Name = "Points",
                IsMapInfoLayer = true,
                DataSource = new MemoryProvider(GetPoints()),
                Style = new SymbolStyle
                {
                    SymbolScale = 0.20
                }
            };
        }

        private static IEnumerable<IFeature> GetPoints()
        {
            var locations = new List<Location>
            {
                new Location
                {
                    Description = "caca",
                    Lat = 46.765887,
                    Lng = 23.598892
                },
                new Location
                {
                    Description = "caca2",
                    Lat = 46.753402,
                    Lng = 23.599013
                }
            };

            return locations.Select(x =>
            {
                var feature = new Feature();
                feature.Geometry = SphericalMercator.FromLonLat(x.Lng, x.Lat);
                feature["description"] = x.Description;
                return feature;
            });
        }

        private class Location
        {
            public string Description { get; set; }
            public double Lat { get; set; }
            public double Lng { get; set; }
        }
    }
}