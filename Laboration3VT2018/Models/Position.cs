using System;
using Windows.Devices.Geolocation;
using Windows.UI;

namespace Laboration3VT2018.Models
{
    [Serializable()]
    public class Position
    {
        public Geoposition GeoPosition { get; set; }
        public bool IsVisited { get; set; }
        public float VisitSpeed { get; set; }
        public DateTime VisitedTime { get; set; }
        public Color Color { get; set; }
        public float DistanceToEnd { get; set; }
        public bool IsMonitoredPosition { get; set; }
    }
}