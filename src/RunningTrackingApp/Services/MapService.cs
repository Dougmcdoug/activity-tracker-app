using BruTile.Predefined;
using Mapsui.Tiling.Layers;
using Mapsui;
using Mapsui.UI.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningTrackingApp.Models;
using Mapsui.Projections;
using NetTopologySuite.Geometries;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Styles;
using System.Windows.Controls;

namespace RunningTrackingApp.Services
{
    public class MapService
    {
        private MapControl _control = new MapControl();

        public Map Map { get; private set; }

        public MapService()
        {
            Map = new Map();
        }


        /// <summary>
        /// Load map by creating a tile layer and adding to the map object.
        /// </summary>
        public void LoadMap()
        {
            // Create tile layer
            var tileLayer = new TileLayer(KnownTileSources.Create());

            // Add tile layer
            Map.Layers.Add(tileLayer);
        }


        /// <summary>
        /// Create a LineString from the gps points and overlay on to the map.
        /// </summary>
        public void UpdateGpsLayer(Coordinate[] points)
        {
            // If we have no GPS trace to plot, then just return - having done nothing
            if (points == null || points.Length == 0) return;

            // Convert GPS points to a MapsUI LineString
            var lineString = CreateLineString(points);

            // Display trace
            DisplayGpsTrace(lineString);
        }


        /// <summary>
        /// Create LineString from coordinate array.
        /// This method is not strictly necessary due to refactoring but has been left in place.
        /// </summary>
        /// <param name="coordinates"></param>
        /// <returns></returns>
        public LineString CreateLineString(Coordinate[] coordinates)
        {
            // Return a new Linestring constructed from the projected coordinates.
            return new LineString(coordinates);
        }


        /// <summary>
        /// Converts list of TrackPoint (representing longitude/latitude coordinates in EPSG:4326) to 
        /// a Coordinate array (representing x, y coordinates in EPSG:3857)
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public Coordinate[] GetCoordinateArray(List<TrackPoint> points)
        {
            // Convert the list of TrackPoints to a coordinate
            // Note that Mapsui expects coordinates in EPSG:3857 (Spherical Mercator projection coordinate system).
            // Gpx files provide data points in EPSG:4326 (longitude/latitude coordinates).
            // We can use the SphericalMercator class to convert between them.
            var coordinates = points.Select(p =>
            {
                var projected = SphericalMercator.FromLonLat(p.Longitude, p.Latitude);
                return new Coordinate(projected.x, projected.y);
            });

            return coordinates.ToArray();
        }


        /// <summary>
        /// Create the gps trace on the map and refresh to display.
        /// </summary>
        /// <param name="lineString">A LineString representing the gps trace to plot.</param>
        public void DisplayGpsTrace(LineString lineString)
        {
            // Create a feature and set its geometry
            var feature = new GeometryFeature { Geometry = lineString };

            // Define the line style
            var vectorStyle = new VectorStyle
            {
                Line =
                    {
                        Color = Mapsui.Styles.Color.Red,
                        Width = 2
                    }
            };

            feature.Styles.Add(vectorStyle);

            // Create a layer to hold the line
            var lineLayer = new MemoryLayer("GPX Track")
            {
                Style = vectorStyle,
                Features = new List<IFeature> { feature }
            };

            // Add to map
            //_control.Map.Layers.Add(lineLayer);
            Map.Layers.Add(lineLayer);
        }

        /// <summary>
        /// Centre the map on a point centre with the zoom set by the resolution.
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="resolution"></param>
        public void FocusOnPoint(MPoint centre, double resolution)
        {
            // Clamp the resolution if it lies outside the allowable range
            //var clampedResolution = Math.Clamp(resolution, Control.Map.Navigator.ZoomBounds.Min, Control.Map.Navigator.ZoomBounds.Max);
            var clampedResolution = Math.Clamp(resolution, Map.Navigator.ZoomBounds.Min, Map.Navigator.ZoomBounds.Max);

            // Focus the map view on this point
            // Delay is required to ensure that the map has loaded before attempting to zoom
            Task.Delay(300).ContinueWith(_ =>
            {
                //_control.Map.Navigator.CenterOnAndZoomTo(centre, clampedResolution);
                Map.Navigator.CenterOnAndZoomTo(centre, clampedResolution);
            });
        }
    }
}
