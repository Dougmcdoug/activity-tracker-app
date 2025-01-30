using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BruTile.Predefined;
using Mapsui;
//using Mapsui.Geometries;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using RunningTrackingApp.Models;

namespace RunningTrackingApp.ViewModels
{
    public class GPSTraceViewModel : ViewModelBase
    {
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


        private List<TrackPoint> _points;

        public GPSTraceViewModel()
        {
            LoadMap();
        }

        private void LoadMap()
        {
            var tileLayer = new TileLayer(KnownTileSources.Create());
            var map = new Map();
            map.Layers.Add(tileLayer);
            Map = map;
        }
    }
}
