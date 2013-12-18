using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Helpers.Serializable.Weapons
{
    [Serializable()]
    public class WeaponBox
    {
        public float Width { get; set; }
        public float Heigth { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }

        public long Tag { get; set; }
        public WeaponBox(float w, float h, Vector3 position, long Tag) 
        {
            this.Width = w;
            this.Heigth = h;
            this.PosX = position.X;
            this.PosY = position.Y;
            this.PosZ = position.Z;
            this.Tag = Tag;
        }
    }
}
