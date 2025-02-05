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

        
    }


    
}
