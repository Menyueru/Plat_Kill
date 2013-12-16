using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameLauncher.SerializableObjects.Preferences
{
    [Serializable()]
    public class PreferenceSerializable
    {        
        [XmlElement("MasterVolume")]
        public int MasterVolume { get; set; }

        [XmlElement("Fullscreen")]
        public bool FullScreen { get; set; }

        [XmlElement("ResolutionWidth")]        
        public int ResolutionWidth { get; set; }

        [XmlElement("ResolutionHeigth")]        
        public int ResolutionHeight { get; set; }
    }
}
