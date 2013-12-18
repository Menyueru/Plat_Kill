using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace plat_kill.Networking.Messages
{
    public class TimeUpdate : IGameMessage
    {
        #region Propierties
        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.TimeUpdate; }
        }

        public Tuple<DateTime, TimeSpan> Time { get; set; }

        #endregion  

        public TimeUpdate(NetIncomingMessage im) 
        {
            this.Decode(im);
        }

        public TimeUpdate(DateTime dt,TimeSpan ts) 
        {
            this.Time = new Tuple<DateTime, TimeSpan>(dt, ts);
        }
 
        #region Methods
        public void Decode(NetIncomingMessage im)
        {
            if(this.Time == null)
            {
                this.Time = new Tuple<DateTime, TimeSpan>(new DateTime(), new TimeSpan());
            }
            int length = im.ReadInt32();
            byte[] dataFromServer = im.ReadBytes(length);

            this.Time = (Tuple<DateTime, TimeSpan>)ByteArrayToObject(dataFromServer);
        }

        public void Encode(NetOutgoingMessage om)
        {
            byte[] data = ObjectToByteArray(this.Time);

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
