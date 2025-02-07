using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using RunningTrackingApp.Interfaces;
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

        /// <summary>
        /// Retrieve a list of elevation values from a list of TrackPoints.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private List<double> GetElevation(List<TrackPoint> points)
        {
            // Check that elevation is not null
            List<double> elevation = points.
                Select(p => p.Elevation ?? throw new ArgumentNullException("Elevation value is null."))
                .ToList();

            return elevation;
        }

        /// <summary>
        /// Calculate the total elevation gain from a list of TrackPoints.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public double CalculateTotalElevation(List<TrackPoint> points)
        {
            // Perform smoothing
            var smoothedPoints = PerformSmoothing(points);

            // Get a list of the elevation values from the smoothed points
            var elevations = GetElevation(smoothedPoints);

            // Iterate through points to determine elevation change
            double totalElevationGain = 0;
            double totalElevationLoss = 0;
            for (int i = 0; i < elevations.Count - 1; i++)
            {
                var elevationChange = elevations[i + 1] - elevations[i];

                // If elevation changes by less than 10cm, we can discount it as noise
                if (elevationChange >= 0.1)
                {
                    totalElevationGain += elevationChange;
                }
                else if (elevationChange <= -0.1)
                {
                    totalElevationLoss -= elevationChange;
                }
            }

            return totalElevationGain;
        }


        /// <summary>
        /// Perform smoothing on a list of TrackPoints.
        /// The 3D Kalman filter is definitely overkill for this application, a moving average filter on the elevation would likely
        /// be sufficient but it provided a good opportunity to have a go at implementing this filter!
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public List<TrackPoint> PerformSmoothing(List<TrackPoint> points)
        {
            // Initialise list
            var smoothedPoints = new List<TrackPoint>();

            // Initialise the filter
            // Could swap in other filter types here, such as a simpler moving average filter.
            // Could do this by the choice in a UI drop-down list in a settings menu?
            IGpsFilter filter = new KalmanFilter3D(points[0].Latitude, points[0].Longitude, (double)points[0].Elevation);

            // Iterate through points and smooth them by updating and retrieving the state of the filter
            foreach (var point in points)
            {
                filter.Update(point.Latitude, point.Longitude, (double)point.Elevation);
                var smoothed = filter.GetState();
                smoothedPoints.Add(new TrackPoint { Latitude = smoothed.x, Longitude = smoothed.y, Elevation = smoothed.z });

            }

            return smoothedPoints;
        }
        
    }


    
}
