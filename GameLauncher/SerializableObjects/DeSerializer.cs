using GameLauncher.SerializableObjects.Characters;
using GameLauncher.SerializableObjects.Maps;
using GameLauncher.SerializableObjects.Preferences;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

        public static PreferenceCollection DeserializePreferencesCollection(string xmlPath)
        {
            PreferenceCollection tempCollection = null;

            XmlSerializer serializer = new XmlSerializer(typeof(PreferenceCollection));

            StreamReader reader = new StreamReader(xmlPath);
            tempCollection = (PreferenceCollection)serializer.Deserialize(reader);
            reader.Close();

            return tempCollection;
        }

        public static void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }


    }
}
