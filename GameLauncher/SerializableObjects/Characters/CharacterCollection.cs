using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameLauncher.SerializableObjects.Characters
{
    [Serializable()]
    [XmlRoot("CharacterCollection")]
    public class CharacterCollection
    {
        [XmlArray("Characters")]
        [XmlArrayItem("Character", typeof(CharacterSerializable))]
        public CharacterSerializable[] Characters { get; set; }
    }
}
