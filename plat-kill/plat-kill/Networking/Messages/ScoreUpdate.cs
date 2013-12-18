using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace plat_kill.Networking.Messages
{
    public class ScoreUpdate : IGameMessage
    {
        #region Propierties
        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.ScoreUpdate; }
        }

        public Dictionary<long, int> Score{get;set;}
        #endregion  

        public ScoreUpdate() { }

        public ScoreUpdate(NetIncomingMessage im)
        {
            this.Decode(im);
        }

        public ScoreUpdate(Dictionary<long, int> Score)
        {
            this.Score = Score;
        }

        #region Methods
        public void Decode(NetIncomingMessage im)
        {
            if(this.Score == null)
            {
                this.Score = new Dictionary<long, int>();
            }

            int length = im.ReadInt32();
            byte[] dataFromServer = im.ReadBytes(length);

            this.Score = (Dictionary<long, int>)ByteArrayToObject(dataFromServer);
        }

        public void Encode(NetOutgoingMessage om)
        {
            byte[] data = ObjectToByteArray(this.Score);

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

        private object ByteArrayToObject(byte[] data)
        {
            if (data.Length < 0 || data == null)
            {
                return null;
            }
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream(data);

            return bf.Deserialize(ms);
        }
        #endregion 
    }
}
