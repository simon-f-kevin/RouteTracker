using System;
using Windows.UI;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.Devices.Geolocation;

namespace Laboration3VT2018.Models
{
    [Serializable()]
    public class Position
    {
        public BasicGeoposition Point { get; set; }
        public bool IsVisited { get; set; }
        public float VisitSpeed { get; set; }
        public DateTime VisitedTime { get; set; }
        public Color Color { get; set; }
        public double LengthToEndPoint { get; set; }
        public bool IsMonitoredPosition { get; set; }
    }
}