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
    public class GPSTraceViewModel : ViewModelBase, INavigable
    {
        public string FilePath { get; private set; }
        private GPXParserService _parserService;
        private GPXProcessorService _processorService;
        private MapService _mapService;
        private List<TrackPoint> _points;
        private Coordinate[] _coordinates;
        private List<TrackPoint> _smoothedPoints;

        private Map _map;
        public Map Map
        {
            get { return _map; }
            set { SetProperty(ref _map, value); }
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
        /// <param name="mapService"></param>
        public GPSTraceViewModel(GPXParserService parserService, GPXProcessorService processorService, MapService mapService)
        {
            _parserService = parserService; 
            _processorService = processorService;
            _mapService = mapService;

            Map = _mapService.Map;
        }


        public void OnNavigatedTo(object parameter = null)
        {
            if (parameter is string filePath)
            {
                // Load the map
                _mapService.LoadMap();

                try
                {
                    // Import the gpx data from the selected file and overlay it on the map
                    LoadGPXTrack(filePath);

                    // Zoom map on the gps trace
                    FocusMapOnTrace();
                }
                catch (ArgumentException ex)
                {
                    // Ignore exception, map will proceed to draw map without gps trace
                    // Could add a dialog box warning the user no gpx file was selected
                }
            }
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

            // Smooth data points
            _smoothedPoints = _processorService.PerformSmoothing(_points);

            // Convert lon/lat coordinates to x,y
            _coordinates = _mapService.GetCoordinateArray(_smoothedPoints);

            // Update the map with the new GPS track
            _mapService.UpdateGpsLayer(_coordinates);
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
            var centre = new MPoint(minX + (maxX - minX) / 2, minY + (maxY - minY) / 2);

            // Calculate span
            var spanX = maxX - minX;
            var spanY = maxY - minY;
            var maxSpan = Math.Max(spanX, spanY);

            // Calculate the zoom level
            // The Log2() is used because a larger span means we need a smaller resolution level.
            // The hardcoded correction is an empirically determined correction that produces a good zoom.
            double zoomCorrection = 25; // "Magic" value that produces a good zoom level
            double resolution = maxSpan > 0 ? Math.Log2(360 / maxSpan) + zoomCorrection : 10;

            // Focus the map view on this point
            // Delay is required to ensure that the map has loaded before attempting to zoom
            _mapService.FocusOnPoint(centre, resolution);
        }


    }
}
