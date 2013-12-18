using System;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using plat_kill.GameModels.Weapons;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using plat_kill.Helpers.Serializable.Weapons;
using System.IO.Compression;

namespace plat_kill.Networking.Messages
{
    public class WeaponUpdateMessage : IGameMessage
    {
        #region Propierties
        public Dictionary<long, Tuple<long, WeaponBox>> activeWeapons { get; set; }

        public GameMessageTypes MessageType
        {
            get
            {
                return GameMessageTypes.WeaponStateChange;
            }
        }
        #endregion 

        public WeaponUpdateMessage(NetIncomingMessage im) 
        {
            this.Decode(im);
        }

        public WeaponUpdateMessage(Dictionary<long, Tuple<Weapon, Box>> activeWeapons) 
        {
            this.activeWeapons = this.WeaponCollectionToMessageCollection(activeWeapons);
        }

        public void Decode(NetIncomingMessage im)
        {
            if(this.activeWeapons == null)
            {
                this.activeWeapons = new Dictionary<long, Tuple<long, WeaponBox>>();
            }
            int length = im.ReadInt32();
            Console.WriteLine(length);
            byte[] dataFromServer = im.ReadBytes(length);

            this.activeWeapons = (Dictionary<long, Tuple<long, WeaponBox>>)ByteArrayToObject(dataFromServer);
        }

        public void Encode(NetOutgoingMessage om)
        {
            byte[] data = ObjectToByteArray(this.activeWeapons);

            om.Write(data.Length);
            om.Write(data);
        }

        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            
            return ms.ToArray();
        }

        private object ByteArrayToObject(byte [] data) 
        {
            if(data.Length < 0 || data == null)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(data);
                
            return bf.Deserialize(ms);
        }

        private Dictionary<long, Tuple<long, WeaponBox>> WeaponCollectionToMessageCollection(Dictionary<long, Tuple<Weapon, Box>> activeWeapons) 
        {
            Dictionary<long, Tuple<long, WeaponBox>> tempDic = new Dictionary<long, Tuple<long, WeaponBox>>();
            foreach (var element in activeWeapons)
            {
                tempDic.Add(element.Key, new Tuple<long, WeaponBox>(element.Value.Item1.WeaponID,
                                                                        new WeaponBox(element.Value.Item2.Width,
                                                                        element.Value.Item2.Height,
                                                                        element.Value.Item2.Position,
                                                                        Convert.ToInt64(element.Value.Item2.Tag))));
            }

            return tempDic;
        }
    }
}
