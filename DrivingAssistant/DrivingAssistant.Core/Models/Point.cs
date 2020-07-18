using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace DrivingAssistant.Core.Models
{
    public class Point
    {
        [JsonProperty("X")]
        public float X;

        [JsonProperty("Y")]
        public float Y;

        //===========================================================//
        [JsonConstructor]
        public Point(float x, float y)
        {
            X = x;
            Y = y;
        }

        //===========================================================//
        public Point(double x, double y)
        {
            X = Convert.ToSingle(x);
            Y = Convert.ToSingle(y);
        }
    }

    public static class PointExtensions
    {
        //===========================================================//
        public static string PointToString(this Point point)
        {
            return point.X.ToString(CultureInfo.InvariantCulture) + "," + point.Y.ToString(CultureInfo.InvariantCulture);
        }

        //===========================================================//
        public static Point StringToPoint(this string str)
        {
            var x = Convert.ToSingle(str.Split(',')[0]);
            var y = Convert.ToSingle(str.Split(',')[1]);
            return new Point(x, y);
        }

        //===========================================================//
        public static string PointCollectionToString(this ICollection<Point> points)
        {
            return string.Join(";", points.Select(point => point.PointToString()));
        }

        //===========================================================//
        public static ICollection<Point> StringToPointCollection(this string str)
        {
            if (str == string.Empty)
            {
                return new List<Point>();
            }
            var elements = str.Split(';');
            if (elements.Length == 0)
            {
                return new List<Point>();
            }
            return elements.Select(point => point.StringToPoint()).ToList();
        }
    }
}