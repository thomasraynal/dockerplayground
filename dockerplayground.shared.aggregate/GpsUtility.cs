using System;
using System.Collections.Generic;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public static class GpsUtilities
    {

        private static double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        public static int X = 50;
        public static int Y = 50;
        public static int Radius = 5000000;

        //https://gis.stackexchange.com/questions/25877/generating-random-locations-nearby
        public static LocationRecord GetLocation()
        {
            Random random = new Random();

            // Convert radius from meters to degrees
            double radiusInDegrees = (int)C_EARTH / 111000f;

            var u = DemoHelper.Rand.NextDouble();
            var v = DemoHelper.Rand.NextDouble();
            var w = radiusInDegrees * Math.Sqrt(u);
            var t = 2 * Math.PI * v;
            var x = w * Math.Cos(t);
            var y = w * Math.Sin(t);

            // Adjust the x-coordinate for the shrinking of the east-west distances
            var new_x = x / Math.Cos(ConvertToRadians(Y));

            var foundLongitude = new_x + X;
            var foundLatitude = y + Y;

            return new LocationRecord()
            {
                Latitude = (float)foundLatitude,
                Longitude = (float)foundLongitude,
            };
     
        }

        private const double C_EARTH = 40000.0;

        public static double DegToRad(double angle)
        {
            return (Math.PI / 180) * angle;
        }


        //https://github.com/microservices-aspnetcore/es-eventprocessor/blob/42852a1d70daf36e88011eb4998e5e98aa07977e/src/StatlerWaldorfCorp.EventProcessor/Location/GpsUtility.cs
        public static double DistanceBetweenPoints(PositionRecord point1, PositionRecord point2)
        {
            double distance = 0.0;


            double lat1Rad = DegToRad(point1.Latitude);
            double long1Rad = DegToRad(point1.Longitude);
            double lat2Rad = DegToRad(point2.Latitude);
            double long2Rad = DegToRad(point2.Longitude);

            double longDiff = Math.Abs(long1Rad - long2Rad);

            if (longDiff > Math.PI)
            {
                longDiff = 2.0 * Math.PI - longDiff;
            }

            double angleCalculation =
                Math.Acos(
                    Math.Sin(lat2Rad) * Math.Sin(lat1Rad) +
                    Math.Cos(lat2Rad) * Math.Cos(lat1Rad) * Math.Cos(longDiff));

            distance = C_EARTH * angleCalculation / (2.0 * Math.PI);

            return distance;
        }
    }
}
