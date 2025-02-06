using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using RunningTrackingApp.Helpers;
using RunningTrackingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningTrackingApp.Services
{
    /// <summary>
    /// Service to handle processing of GPS points. This includes calculating summary data (elevation, distance etc.) and the necessary processing.
    /// </summary>
    public class GPXProcessorService
    {
        private readonly MapService _mapService;

        public GPXProcessorService(MapService mapService) 
        { 
            _mapService = mapService;
        }

        /// <summary>
        /// Calculate the total distance of a gpx track. The List of points must have their coordinates
        /// in lon/lat form, not x/y coordinates.
        /// The distance does not account for changes in elevation, and the calculation does not account for
        /// earth being elliptical so accuracy may vary.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public double CalculateTotalDistance(List<TrackPoint> points)
        {
            // Iterate through points and calculate distances
            double totalDistance = 0;
            for (int i = 0; i < points.Count - 1; i++)
            {
                totalDistance += GeoUtils.Haversine(points[i], points[i + 1]);
            }

            return totalDistance;
        }


        private List<double> GetElevation(List<TrackPoint> points)
        {
            // Check that elevation is not null
            List<double> elevation = points.
                Select(p => p.Elevation ?? throw new ArgumentNullException("Elevation value is null."))
                .ToList();

            return elevation;
        }


        public double CalculateTotalElevation(List<TrackPoint> points)
        {
            // Perform smoothing?
            var smoothedPoints = new List<TrackPoint>();
            var kalman = new KalmanFilter3D(points[0].Latitude, points[0].Longitude, (double)points[0].Elevation);
            foreach (var point in points)
            {
                kalman.Update(point.Latitude, point.Longitude, (double)point.Elevation);
                var smoothed = kalman.GetState();
                smoothedPoints.Add(new TrackPoint { Longitude = smoothed.x, Latitude = smoothed.y, Elevation = smoothed.z });

            }

            var elevations = GetElevation(smoothedPoints);

            // Iterate through points to determine elevation
            double totalElevationGain = 0;
            double totalElevationLoss = 0;
            for (int i = 0; i < smoothedPoints.Count - 1; i++)
            {
                var elevationChange = (double)smoothedPoints[i + 1].Elevation - (double)smoothedPoints[i].Elevation;
                if (elevationChange >= 0)
                {
                    totalElevationGain += elevationChange;
                }
                else
                {
                    totalElevationLoss -= elevationChange;
                }

            }
            /*
            foreach (var pt in smoothedPoints)
            {
                totalElevation += (double)pt.Elevation;
            }*/


            return totalElevationGain;
        }

        
    }


    
}
