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

namespace RunningTrackingApp.ViewModels
{
    public class GPSTraceViewModel : ViewModelBase, IParameterReceiver
    {
        public string FilePath { get; private set; }
        private GPXParserService _parserService;
        private List<TrackPoint> _points;

        private MapControl _control;

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

        public GPSTraceViewModel(GPXParserService parserService)
        {
            LoadMap();
            _parserService = parserService;
        }

        private void LoadMap()
        {
            var tileLayer = new TileLayer(KnownTileSources.Create());
            var map = new Map();
            map.Layers.Add(tileLayer);
            Map = map;
        }

        public void LoadGPXTrack(string filePath)
        {
            // Load GPS points
            GPXData = _parserService.ParseGpxFile(filePath);
            _points = _parserService.ExtractTrackPoints(GPXData);

            // Update the map with the new GPS track
            UpdateGpsLayer();
        }

        private void UpdateGpsLayer()
        {
            // If we have no GPS trace to plot, then just return - having done nothing
            if (_points == null || _points.Count == 0) return;

            // Convert GPS points to a MapsUI LineString
            var lineString = CreateLineString(_points);

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
            //var memoryProvider = new MemoryProvider(new[] {feature});
            var lineLayer = new MemoryLayer("GPX Track")
            {
                Style = vectorStyle,
                Features = new List<IFeature> { feature }
            };

            // Add to map
            
        }


        /// <summary>
        /// Create a LineString object from an input list of TrackPoints. The list must be 'flattened'.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private LineString CreateLineString(List<TrackPoint> points)
        {
            var coordinates = points.Select((TrackPoint p) => new Coordinate(p.Longitude, p.Latitude)).ToArray();
            return new LineString(coordinates);
        }


        public void ReceiveParameter(object parameter)
        {
            if (parameter is string filepath)
            {
                FilePath = filepath;
            }
        }
    }
}
