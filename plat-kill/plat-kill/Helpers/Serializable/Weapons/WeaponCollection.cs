using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace plat_kill.Helpers.Serializable.Weapons
{
    [Serializable()]
    [XmlRoot("WeaponCollection")]
    class WeaponCollection
    {
        [XmlArray("Weapons")]
        [XmlArrayItem("Weapon", typeof(SerializableWeapon))]
        public SerializableWeapon[] Weapons { get; set; }
    }
}
