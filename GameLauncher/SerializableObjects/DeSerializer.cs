using GameLauncher.SerializableObjects.Characters;
using GameLauncher.SerializableObjects.Maps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameLauncher.SerializableObjects
{
    public class DeSerializer
    {
        public static CharacterCollection DeserializeCharacterCollection(string xmlPath)
        {
            CharacterCollection tempCollection = null;

            XmlSerializer serializer = new XmlSerializer(typeof(CharacterCollection));

            StreamReader reader = new StreamReader(xmlPath);
            tempCollection = (CharacterCollection)serializer.Deserialize(reader);
            reader.Close();

            return tempCollection;
        }

        public static MapCollection DeserializeMapCollection(string xmlPath)
        {
            MapCollection tempCollection = null;

            XmlSerializer serializer = new XmlSerializer(typeof(MapCollection));
            StreamReader reader = new StreamReader(xmlPath);
            tempCollection = (MapCollection)serializer.Deserialize(reader);
            reader.Close();

            return tempCollection;
        }

    }
}
