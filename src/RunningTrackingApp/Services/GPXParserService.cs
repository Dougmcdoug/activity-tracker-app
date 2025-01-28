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
        /// <summary>
        /// Parse GPX file.
        /// It is expected that only a single track is present in the GPX file.
        /// This is a reasonable assumption because GPS watches and bike computers save only
        /// a single track per GPX file.
        /// </summary>
        /// <param name="filePath">File path to GPX file. Can be relative or absolute.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if invalid filepath is supplied.</exception>
        /// <exception cref="InvalidDataException">Thrown if gpx file contains more than one track. Activities recorded with activity trackers only contain one track,
        /// so it is likely to be generated with a route planner if more than one track is present.</exception>
        public GpxData ParseGpxFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("Invalid gpx file path.");
            }

            GpxData data = new GpxData();

            var doc = XDocument.Load(filePath);

            // Extract attributes from <gpx> tag
            var gpxElement = doc.Root;
            if (gpxElement == null) throw new InvalidDataException("<gpx> element not present in .gpx file.");

            data.Version = gpxElement.Attribute("version")?.Value;
            data.Creator = gpxElement.Attribute("creator")?.Value;

            // Parse tracks
            var ns = gpxElement.GetDefaultNamespace();
            //var tcks = gpxElement.Element(ns + "trk");
            var tracks = gpxElement.Elements(ns + "trk").Select(ParseTrack).ToList();

            data.Tracks = tracks;

            return data;


            // Could optionally process waypoints in the same way
            // Can also come back later to add elevation into this
        }

        private Track ParseTrack(XElement trackElement)
        {
            var ns = trackElement.GetDefaultNamespace();

            return new Track
            {
                ActivityName = trackElement.Element(ns + "name")?.Value,
                ActivityType = trackElement.Element(ns + "type")?.Value,
                Segments = trackElement.Elements(ns + "trkseg").Select(ParseTrackSegment).ToList()
            };
        }

        private TrackSegment ParseTrackSegment(XElement segmentElement)
        {
            var ns = segmentElement.GetDefaultNamespace();

            return new TrackSegment
            {
                Points = segmentElement.Elements(ns + "trkpt").Select(ParseTrackPoint).ToList()
            };
        }

        private TrackPoint ParseTrackPoint(XElement pointElement)
        {
            var ns = pointElement.GetDefaultNamespace();

            return new TrackPoint
            {
                Latitude = double.Parse(pointElement.Attribute("lat")?.Value ?? throw new InvalidDataException("Missing lat attribute")),
                Longitude = double.Parse(pointElement.Attribute("lon")?.Value ?? throw new InvalidDataException("Missing lon attribute")),
                TimeStamp = pointElement.Element(ns + "time") != null
                    ? DateTime.Parse(pointElement.Element(ns + "time").Value)
                    : null
            };
        }
    }
}
