using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameLauncher.SerializableObjects.Characters
{
    [Serializable()]
    public class CharacterSerializable
    {
        [XmlElement("Name")]
        public String Name { get; set; }

        [XmlElement("MeleePower")]
        public long MeleePower { get; set; }

        [XmlElement("RangePower")]
        public long RangePower { get; set; }

        [XmlElement("Health")]
        public long Health { get; set; }

        [XmlElement("Stamina")]
        public long Stamina { get; set; }

        [XmlElement("Defense")]
        public long Defense { get; set; }

        [XmlElement("Speed")]
        public long Speed { get; set; }
    }
}
