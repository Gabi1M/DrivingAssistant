using System.Collections.Generic;
using System.Linq;
using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Projection;
using Mapsui.Providers;
using Mapsui.Styles;

namespace DrivingAssistant.AndroidApp.Tools
{
    public static class MapTools
    {
        //============================================================
        public static MemoryLayer CreatePointLayer(string layerName, Color markerColor, double markerScale, params Point[] points)
        {
            return new MemoryLayer
            {
                Name = layerName,
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
        public static MemoryLayer CreateLineLayer(string layerName, Color lineColor, double lineWidth, params Point[] points)
        {
            var feature = new Feature
            {
                Geometry = new LineString(points),
                Styles = new List<IStyle>
                {
                    new VectorStyle
                    {
                        Line =
                        {
                            Color = lineColor,
                            Width = lineWidth
                        }
                    }
                }
            };
            return new MemoryLayer
            {
                Name = layerName,
                IsMapInfoLayer = true,
                DataSource = new MemoryProvider(feature)
            };
        }

        //============================================================
        public static IEnumerable<Point> ConvertToLonLat(params Point[] points)
        {
            return points.Select(point => SphericalMercator.ToLonLat(point.X, point.Y));
        }

        //============================================================
        public static IEnumerable<Point> ConvertFromLonLat(params Point[] points)
        {
            return points.Select(point => SphericalMercator.FromLonLat(point.X, point.Y));
        }
    }
}