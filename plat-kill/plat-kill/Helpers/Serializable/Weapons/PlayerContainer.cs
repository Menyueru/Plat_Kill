using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Helpers.Serializable.Weapons
{
    [Serializable()]
    public class PlayerContainer
    {
        public long PlayerID { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public PlayerContainer(long id, Vector3 position) 
        {
            this.PlayerID = id;

            this.PosX = position.X;
            this.PosY = position.Y;
            this.PosZ = position.Z;
        }
    }
}
