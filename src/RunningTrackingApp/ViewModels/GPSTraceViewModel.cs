using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BruTile.Predefined;
using Mapsui;
using NetTopologySuite.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using RunningTrackingApp.Interfaces;
using RunningTrackingApp.Models;
using RunningTrackingApp.Services;
using Mapsui.Nts;
using Mapsui.UI.Wpf;
using Mapsui.Projections;
using System.Diagnostics;

namespace RunningTrackingApp.ViewModels
{
    public class GPSTraceViewModel : ViewModelBase
    {
        public string FilePath { get; private set; }
        private GPXParserService _parserService;
        private List<TrackPoint> _points;
        private Coordinate[] _coordinates;

        private MapControl _control;
        public MapControl Control
        {
            get { return _control; }
            set { SetProperty(ref _control, value); }
        }

        private GpxData _gpxData;
        public GpxData GPXData
        {
            get { return _gpxData; }
            set { SetProperty(ref _gpxData, value); }
        }

        /// <summary>
        /// ViewModel contructor. Loads the map so it can be displayed.
        /// </summary>
        /// <param name="parserService"></param>
        public GPSTraceViewModel(GPXParserService parserService)
        {
            _parserService = parserService; 
        }

        /// <summary>
        /// Because we are using Dependency Injection, it isn't straightforward to pass in the
        /// filepath parameter. While it is possible, it is simpler to pass in the filepath as a parameter
        /// to a separate initialising method that should be called after the constructor.
        /// </summary>
        /// <param name="filePath"></param>
        public void Initialise(string filePath)
        {
            // Initialise map control
            Control = new MapControl();

            // Load the map
            LoadMap();

            // Import the gpx data from the selected file and overlay it on the map
            LoadGPXTrack(filePath);

            // Zoom map on the gps trace
            FocusMapOnTrace();
        }

        /// <summary>
        /// Load map and store as the Map property on Control
        /// </summary>
        private void LoadMap()
        {
            // Create tile layer
            var tileLayer = new TileLayer(KnownTileSources.Create());

            // Initialise map
            var map = new Map();

            // Add tile layer
            map.Layers.Add(tileLayer);

            Control.Map = map;
        }

        /// <summary>
        /// Load the gpx file using the parser service and update the map with the gps trace
        /// </summary>
        /// <param name="filePath">Absolute filepath to a valid .gpx file.</param>
        public void LoadGPXTrack(string filePath)
        {
            // Load GPS points
            GPXData = _parserService.ParseGpxFile(filePath);

            // 'Flatten' the gps points into a list
            _points = _parserService.ExtractTrackPoints(GPXData);

            // Update the map with the new GPS track
            UpdateGpsLayer();
        }

        /// <summary>
        /// Create a LineString from the gps points and overlay on to the map.
        /// </summary>
        private void UpdateGpsLayer()
        {
            // If we have no GPS trace to plot, then just return - having done nothing
            if (_points == null || _points.Count == 0) return;

            // Convert GPS points to a MapsUI LineString
            var lineString = CreateLineString(_points);

            // Display trace
            DisplayGpsTrace(lineString);
        }


        /// <summary>
        /// Create a LineString object from an input list of TrackPoints. The list must be 'flattened'.
        /// </summary>
        /// <param name="points">A 'flattened' list of gps points.</param>
        /// <returns></returns>
        private LineString CreateLineString(List<TrackPoint> points)
        {
            // Convert the list of TrackPoints to a coordinate
            // Note that Mapsui expects coordinates in EPSG:3857 (Spherical Mercator projection coordinate system).
            // Gpx files provide data points in EPSG:4326 (longitude/latitude coordinates).
            // We can use the SphericalMercator class to convert between them.
            _coordinates = _points.Select(p =>
            {
                var projected = SphericalMercator.FromLonLat(p.Longitude, p.Latitude);
                return new Coordinate(projected.x, projected.y);
            }).ToArray();


            // Return a new Linestring constructed from the projected coordinates.
            return new LineString(_coordinates);
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
            Control.Map.Layers.Add(lineLayer);

            // Refresh to update the map
            Control.Refresh();
        }

        /// <summary>
        /// Centres and focuses the map on the plotted GPS trace
        /// </summary>
        private void FocusMapOnTrace()
        {
            // Find the extreme x and y values
            var minX = _coordinates.Min(p => p.X);
            var minY = _coordinates.Min(p => p.Y); 
            var maxX = _coordinates.Max(p => p.X);
            var maxY = _coordinates.Max(p => p.Y);

            // Calculate the midpoint to focus on
            var center = new MPoint(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);

            // Calculate span
            var spanX = maxX - minX;
            var spanY = maxY - minY;
            var maxSpan = Math.Max(spanX, spanY);

            // Calculate the zoom level
            // The Log2() is used because a larger span means we need a smaller resolution level.
            // The hardcoded correction is an empirically determined correction that produces a good zoom.
            double zoomCorrection = 25; // "Magic" value that produces a good zoom level
            double resolution = maxSpan > 0 ? Math.Log2(360 / maxSpan) + zoomCorrection : 10;
            var clampedResolution = Math.Clamp(resolution, Control.Map.Navigator.ZoomBounds.Min, Control.Map.Navigator.ZoomBounds.Max);

            // Focus the map view on this point
            // Delay is required to ensure that the map has loaded before attempting to zoom
            Task.Delay(300).ContinueWith(_ =>
            {
                Control.Map.Navigator.CenterOnAndZoomTo(center, clampedResolution);
            });
        }
    }
}
