using System.Collections.Generic;
using Windows.UI;

namespace Laboration3VT2018.Models
{
    public class Route
    {
        public Position StartPosition { get; set; }
        public Position EndPosition { get; set; }
        public List<Position> MarkedPositions { get; set; }
        public Color Color { get; set; }
        public string RouteName { get; set; }
    }
}
