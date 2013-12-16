using plat_kill.GameModels.Projectiles;
using plat_kill.GameModels.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace plat_kill.Helpers.Serializable.Weapons
{
    [Serializable()]
    public class SerializableWeapon
    {
        [XmlElement("Name")]
        public string modelPath { get; set; }
        [XmlIgnore]
        public WeaponType weaponType;
        [XmlElement("WeaponType")]
        public string type
        {
            get
            {
                return EnumToString(weaponType);
            }
            set
            {
                weaponType = StringToEnum<WeaponType>(value);
            }
        }
        [XmlIgnore]
        public ProjectileType projectileType;
        [XmlElement("ProjectileType")]
        public string projtype
        {
            get
            {
                return EnumToString(projectileType);
            }
            set
            {
                projectileType = StringToEnum<ProjectileType>(value);
            }
        }
        [XmlElement("WeaponDamage")]
        public float weaponDamage { get; set; }
        [XmlElement("FireRate")]
        public float fireRate { get; set; }
        [XmlElement("LoadedAmmo")]
        public int loadedAmmo { get; set; }
        [XmlElement("totalAmmo")]
        public int totalAmmo { get; set; }

        private T StringToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        private string EnumToString<T>(T enumValue)
        {
            return enumValue.ToString();
        }
    }
}
