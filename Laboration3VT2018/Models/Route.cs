using System.Collections.Generic;
using Windows.UI;
using System.Xml.Serialization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System;

namespace Laboration3VT2018.Models
{
    [Serializable()]
    public class Route
    {
        public Position StartPosition { get; set; }
        
        public Position EndPosition { get; set; }
        
        public List<Position> MarkedPositions { get; set; }
        
        public Color Color { get; set; }
      
        public string RouteName { get; set; }
    }
}
