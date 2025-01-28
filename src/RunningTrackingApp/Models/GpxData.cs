using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningTrackingApp.Models
{
    public class GpxData
    {

        public string? Version { get; set; }
        public string? Creator { get; set; }
        public List<Waypoint> Waypoints { get; set; } = new List<Waypoint>();
        public List<Track> Tracks { get; set; } = new List<Track>();

    }

    public class Waypoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime TimeStamp { get; set; }
    }

    public class Track
    {
        public string ActivityType { get; set; }
        public string ActivityName { get; set; }
        public List<TrackSegment> Segments { get; set; } = new List<TrackSegment>();
    }

    public class TrackSegment
    {
        public List<TrackPoint> Points { get; set; } = new List<TrackPoint>();
    }

    public class TrackPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime? TimeStamp { get; set; }
    }
}
