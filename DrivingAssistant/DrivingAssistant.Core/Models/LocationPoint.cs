using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class LocationPoint
    {
        [JsonProperty("X")]
        public float X;

        [JsonProperty("Y")]
        public float Y;

        //===========================================================//
        [JsonConstructor]
        public LocationPoint(float x, float y)
        {
            X = x;
            Y = y;
        }

        //===========================================================//
        public LocationPoint(double x, double y)
        {
            X = Convert.ToSingle(x);
            Y = Convert.ToSingle(y);
        }
    }

    public static class LocationPointExtensions
    {
        //===========================================================//
        public static string PointToString(this LocationPoint locationPoint)
        {
            return locationPoint.X.ToString(CultureInfo.InvariantCulture) + "," + locationPoint.Y.ToString(CultureInfo.InvariantCulture);
        }

        //===========================================================//
        public static LocationPoint StringToPoint(this string str)
        {
            var x = Convert.ToSingle(str.Split(',')[0]);
            var y = Convert.ToSingle(str.Split(',')[1]);
            return new LocationPoint(x, y);
        }

        //===========================================================//
        public static string PointCollectionToString(this ICollection<LocationPoint> points)
        {
            return string.Join(";", points.Select(point => point.PointToString()));
        }

        //===========================================================//
        public static ICollection<LocationPoint> StringToPointCollection(this string str)
        {
            if (str == string.Empty)
            {
                return new List<LocationPoint>();
            }
            var elements = str.Split(';');
            return elements.Length == 0 ? new List<LocationPoint>() : elements.Select(point => point.StringToPoint()).ToList();
        }
    }
}