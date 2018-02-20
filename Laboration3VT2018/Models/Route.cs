using System.Collections.Generic;
using Windows.UI;
using System.Xml.Serialization;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Runtime.Serialization;

namespace Laboration3VT2018.Models
{
    [DataContract]
    public class Route
    {
        [DataMember]
        [XmlElement("StartPosition")]
        public Position StartPosition { get; set; }
        [XmlElement("EndPosition")]
        public Position EndPosition { get; set; }
        [XmlElement("MarkedPositions")]
        public List<Position> MarkedPositions { get; set; }
        [XmlElement("Color")]
        public Color Color { get; set; }
        [XmlElement("RouteName")]
        public string RouteName { get; set; }
    }
}
