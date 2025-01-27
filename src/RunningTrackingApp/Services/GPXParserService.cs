using RunningTrackingApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RunningTrackingApp.Services
{
    public class GPXParserService
    {
        public GpxData ParseGpxFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("Invalid gpx file path.");
            }

            var doc = XDocument.Load(filePath);
            GpxData data = new GpxData();

            foreach (var trk in doc.Descendants("trk"))
            {
                var track = new Track();
                foreach (var trkseg in trk.Descendants("trkseg"))
                {
                    var segment = new TrackSegment();
                    foreach (var trkpt in trkseg.Descendants("trkpt"))
                    {
                        var point = new TrackPoint
                        {
                            Latitude = double.Parse(trkpt.Attribute("lat").Value),
                            Longitude = double.Parse(trkpt.Attribute("lon").Value),
                            TimeStamp = DateTime.Parse(trkpt.Element("time")?.Value)
                        };
                        segment.Points.Add(point);
                    }
                    track.Segments.Add(segment);
                }
                data.Tracks.Add(track);
            }

            return data;


            // Could optionally process waypoints in the same way
            // Can also come back later to add elevation into this
        }
    }
}
