using System;
using Lidgren.Network;
using Lidgren.Network.Xna;
using Microsoft.Xna.Framework;
using BEPUphysics.Entities.Prefabs;
using plat_kill.GameModels.Weapons;

namespace plat_kill.Networking.Messages
{
    public class WeaponCreatedMessage : IGameMessage
    {
        #region Propierties
        public long WeaponID { get; set; }
        public Vector3 WeaponPosition { get; set; }
        public double MessageTime { get; set; }
        public bool WasCreated { get; set; }

        public GameMessageTypes MessageType
        {
            get
            {
                return GameMessageTypes.WeaponStateChange;
            }
        }
        #endregion 

        public WeaponCreatedMessage(NetIncomingMessage im) 
        {
            this.Decode(im);
        }

        public WeaponCreatedMessage(Weapon weapon) 
        {
            this.WeaponID = weapon.WeaponID;
            //this.WeaponPosition = box.Position;
            this.MessageTime = NetTime.Now;
            //this.WasCreated = WasCreated;
        }

        public void Decode(NetIncomingMessage im)
        {
            this.WeaponID = im.ReadInt64();
            this.WeaponPosition = im.ReadVector3();
            this.MessageTime = NetTime.Now;
            this.WasCreated = im.ReadBoolean();
        }

        public void Encode(NetOutgoingMessage om)
        {
            om.Write(this.WeaponID);
            om.Write(this.WeaponPosition);
            om.Write(this.MessageTime);
            om.Write(this.WasCreated);
        }
    }
}
