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
            // Throw if file does not exist
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("Invalid gpx file path.");
            }

            GpxData data = new GpxData();

            // Read .gpx file
            var doc = XDocument.Load(filePath);

            // Extract attributes from <gpx> tag
            var gpxElement = doc.Root;
            if (gpxElement == null) 
                throw new InvalidDataException("<gpx> element not present in .gpx file.");

            data.Version = gpxElement.Attribute("version")?.Value;
            data.Creator = gpxElement.Attribute("creator")?.Value;

            // Parse tracks - note: LINQ method syntax
            var ns = gpxElement.GetDefaultNamespace();
            var tracks = gpxElement.Elements(ns + "trk").Select(ParseTrack).ToList();

            data.Tracks = tracks;

            return data;


            // Could optionally process waypoints in the same way
            // Can also come back later to add elevation into this
        }

        /// <summary>
        /// Parses Track element for ActivityName, ActivityType and Segments.
        /// </summary>
        /// <param name="trackElement">Track element to parse.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Parses segment for Track segments.
        /// </summary>
        /// <param name="segmentElement">Segment element to parse</param>
        /// <returns></returns>
        private TrackSegment ParseTrackSegment(XElement segmentElement)
        {
            var ns = segmentElement.GetDefaultNamespace();

            return new TrackSegment
            {
                Points = segmentElement.Elements(ns + "trkpt").Select(ParseTrackPoint).ToList()
            };
        }

        /// <summary>
        /// Parses point element for all Latitude, Longitude and Timestamp. Optional Elevation and Extension elements
        /// are checked, and relevant properties are set to null if they are not present.
        /// </summary>
        /// <param name="pointElement">Point element to parse.</param>
        /// <returns>Trackpoint object.</returns>
        /// <exception cref="InvalidDataException"></exception>
        private TrackPoint ParseTrackPoint(XElement pointElement)
        {
            var ns = pointElement.GetDefaultNamespace();

            var extensionData = pointElement.Element(ns + "extensions") != null
                ? ParseExtensions(pointElement.Element(ns + "extensions"))
                : new ExtensionData();
            

            return new TrackPoint
            {
                Latitude = double.Parse(pointElement.Attribute("lat")?.Value ?? throw new InvalidDataException("Missing lat attribute")),
                Longitude = double.Parse(pointElement.Attribute("lon")?.Value ?? throw new InvalidDataException("Missing lon attribute")),
                TimeStamp = pointElement.Element(ns + "time") != null
                    ? DateTime.Parse(pointElement.Element(ns + "time").Value)
                    : null,
                Elevation = pointElement.Element(ns + "ele") != null
                    ? double.Parse(pointElement.Element(ns + "ele").Value)
                    : null,
                HeartRate = extensionData.HeartRate,
                Power = extensionData.Power,
                Cadence = extensionData.Cadence
            };
        }

        /// <summary>
        /// Parse Extensions element for optional Power and "gpxtpx:TrackPointExtension" elements. If not present,
        /// relevant properties are set to null.
        /// </summary>
        /// <param name="extensionElement">Extensions element to parse.</param>
        /// <returns>ExtensionData object.</returns>
        private ExtensionData ParseExtensions(XElement extensionElement)
        {
            var ns = extensionElement.GetDefaultNamespace();
            var gpxtpx = extensionElement.GetNamespaceOfPrefix("gpxtpx");

            var trackPointExtensionData = extensionElement.Element(gpxtpx + "TrackPointExtension") != null
                ? ParseTrackPointExtension(extensionElement.Element(gpxtpx + "TrackPointExtension"))
                : new TrackPointExtensionData();

            return new ExtensionData
            {
                Power = extensionElement.Element(ns + "power") != null
                    ? int.Parse(extensionElement.Element(ns + "power")?.Value)
                    : null,
                HeartRate = trackPointExtensionData.HeartRate,
                Cadence = trackPointExtensionData.Cadence
            };

        }

        /// <summary>
        /// Parses gpxtpx:TrackPointExtension element for optional HeartRate and Cadence values. Returns
        /// null for values not present.
        /// </summary>
        /// <param name="trackPointExtensionElement">gpxtpx:TrackPointExtension element to parse.</param>
        /// <returns>TrackPointExtensionData object.</returns>
        private TrackPointExtensionData ParseTrackPointExtension(XElement trackPointExtensionElement) {
            var gpxtpx = trackPointExtensionElement.GetNamespaceOfPrefix("gpxtpx");

            return new TrackPointExtensionData
            {
                HeartRate = trackPointExtensionElement.Element(gpxtpx + "hr") != null
                    ? int.Parse(trackPointExtensionElement.Element(gpxtpx + "hr")?.Value)
                    : null,
                Cadence = trackPointExtensionElement.Element(gpxtpx + "cad") != null
                    ? int.Parse(trackPointExtensionElement.Element(gpxtpx + "cad")?.Value)
                    : null
            };
        }
    }

    class ExtensionData
    {
        public int? Power { get; set; } = null;
        public int? Cadence { get; set; } = null;
        public int? HeartRate { get; set; } = null;
    }

    class TrackPointExtensionData
    {
        public int? Cadence { get; set; } = null;
        public int? HeartRate { get; set; } = null;
    }
}
