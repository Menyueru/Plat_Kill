using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameLauncher.SerializableObjects.Maps
{
    [Serializable()]
    public class MapSeriazable
    {
        [XmlElement("Name")]
        public String Name { get; set; }

        [XmlElement("Model")]
        public String Model { get; set; }
    }
}
