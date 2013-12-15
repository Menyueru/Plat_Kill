using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameLauncher.SerializableObjects.Maps
{
    [Serializable()]
    [XmlRoot("MapCollection")]
    public class MapCollection
    {
        [XmlArray("Maps")]
        [XmlArrayItem("Map", typeof(MapSeriazable))]
        public MapSeriazable[] Maps { get; set; }
    }
}
