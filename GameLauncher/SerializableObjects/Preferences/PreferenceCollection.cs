using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameLauncher.SerializableObjects.Preferences
{
    [Serializable()]
    [XmlRoot(ElementName = "PreferenceCollection")]
    public class PreferenceCollection
    {
        [XmlArray("Preferences")]
        [XmlArrayItem("Preference", typeof(PreferenceSerializable))]
        public PreferenceSerializable[] Preferences { get; set; }
    }
}
