using RunningTrackingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningTrackingApp.Helpers
{
    public static class GeoUtils
    {
        /// <summary>
        /// Implementation of the Haversine Formula to determine the shortest geometrical distance between two TrackPoints
        /// with lon/lat coordinates around a sphere with the mean radius of the earth.
        /// Note that this does not account for the earth being elliptical, and does not account for changes in elevation.
        /// https://en.wikipedia.org/wiki/Haversine_formula
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        public static double Haversine(TrackPoint point1, TrackPoint point2)
        {
            // Earth mean radius in metres
            const double r = 6371e3; 

            // Get longitude and latitude coordinates from TrackPoints
            var lon1 = DegreesToRadians(point1.Longitude);
            var lon2 = DegreesToRadians(point2.Longitude);
            var lat1 = DegreesToRadians(point1.Latitude);
            var lat2 = DegreesToRadians(point2.Latitude);

            // Calculate intermediate parameters. Equal to delta phi/delta lambda in the wikipedia article
            var dlat = (lat2 - lat1) / 2;
            var dlon = (lon2 - lon1) / 2;

            // Run required trig calculations - so each needs to be evaluated only once
            var sin_dlat = Math.Sin(dlat);
            var sin_dlon = Math.Sin(dlon);
            var cos_lat1 = Math.Cos(lat1);
            var cos_lat2 = Math.Cos(lat2);

            // Following the logic in the wikipedia article applying the arcsine to find theta...
            var hav_theta = sin_dlat*sin_dlat + (cos_lat1 * cos_lat2 * sin_dlon*sin_dlon);

            // Arctangent form is acceptable because it does not suffer from significant numerical errors when the
            // distance between points is small.
            // This would only be an issue if points were on the opposite side of the sphere, something we would
            // not expect from two consecutive points on a GPS track!
            var theta = 2 * Math.Atan2(Math.Sqrt(hav_theta), Math.Sqrt(1 - hav_theta));

            // Use the radius and central angle to determine the shortest distance d
            var d = r * theta;
            return d;
        }

        /// <summary>
        /// Convert an input angle from degrees to radians.
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public static double DegreesToRadians(double deg)
        {
            return deg * Math.PI / 180;
        }
    }

    
}
