using Lidgren.Network;
using plat_kill.GameModels.Players;
using plat_kill.Helpers.Serializable.Weapons;
using plat_kill.Helpers.States;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace plat_kill.Networking.Messages
{
    public class NewPlayerJoined : IGameMessage
    {
        #region Fields
        public GameMessageTypes MessageType
        {
            get { return GameMessageTypes.NewPlayerJoined; }
        }

        public List<PlayerContainer> Players { get; set; }
        public Maps Map { get; set; }
        #endregion


        public NewPlayerJoined() { }

        public NewPlayerJoined(NetIncomingMessage im) 
        {
            this.Decode(im);
        }

        public NewPlayerJoined(List<Player> players)
        {
            this.Players = new List<PlayerContainer>();
            foreach (Player p in players)
            {
                Players.Add(new PlayerContainer(p.Id, p.Position));
            }
        }

        public NewPlayerJoined(List<Player> players, Maps map) 
        {
            this.Map = map;
            this.Players = new List<PlayerContainer>();
            foreach(Player p in players)
            {
                Players.Add(new PlayerContainer(p.Id, p.Position));
            }
        }

        #region Methods
        public void Decode(NetIncomingMessage im)
        {
            if(this.Players == null || this.Map == null)
            {
                this.Players = new List<PlayerContainer>();
                this.Map = new Maps();
            }
            this.Map = (Maps)Enum.Parse(typeof(Maps), im.ReadString());

            int length = im.ReadInt32();
            byte[] dataFromServer = im.ReadBytes(length);

            this.Players = (List<PlayerContainer>)ByteArrayToObject(dataFromServer);
        }

        public void Encode(NetOutgoingMessage om)
        {
            byte[] data = ObjectToByteArray(this.Players);

            om.Write(this.Map.ToString());
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
