using System;
using System.Collections.Generic;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.CollisionRuleManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace plat_kill.GameModels.Projectiles
{
    public class Projectile : StaticModel
    {
        #region Properties
        private Sphere body;
        private ProjectileType projectileType;

        private long projectileID;
        private long firedByPlayerID;
        private float radius;
        private float speed;
        private int colisiontime=0;

        private bool removeFromSpace = false;
        
        #endregion

        #region Getter-Setter
        
        public int Colisiontime
        {
            get { return colisiontime; }
            set { colisiontime = value; }
        }

        public bool RemoveFromSpace
        {
            get { return removeFromSpace; }
            set { removeFromSpace = value; }
        }


        public long ProjectileID
        {
            get { return projectileID; }
            set { projectileID = value; }
        }

        public ProjectileType ProjectileType
        {
            get { return projectileType; }
            set { projectileType = value; }
        }

        public long FiredByPlayerID
        {
            get { return firedByPlayerID; }
            set { firedByPlayerID = value; }
        }

        public Sphere Body
        {
            get { return body; }
            set { body = value; }
        }

        public float Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        #endregion

        #region Methods
        private float CalculateRadius()
        {
            float maxRadius=float.MinValue;
            foreach (var mesh in Model.Meshes)
            {
                foreach (var part in mesh.MeshParts)
                {
                    int stride = part.VertexBuffer.VertexDeclaration.VertexStride;
                    byte[] vertexData = new byte[stride * part.NumVertices];
                    part.VertexBuffer.GetData(part.VertexOffset * stride, vertexData, 0, part.NumVertices, 1);
                    for (int ndx = 0; ndx < vertexData.Length; ndx += stride)
                    {
                        float x = Math.Abs(BitConverter.ToSingle(vertexData, ndx) - Position.X);
                        float y = Math.Abs(BitConverter.ToSingle(vertexData, ndx + sizeof(float)) - Position.Y);
                        float z = Math.Abs(BitConverter.ToSingle(vertexData, ndx + sizeof(float) * 2) - Position.Z);
                        float xoffset = Math.Abs(x - Position.X);
                        float yoffset = Math.Abs(y - Position.Y); ;
                        float zoffset = Math.Abs(z - Position.Z); ;
                        float tempMax = Math.Max(yoffset, xoffset);
                        tempMax = Math.Max(tempMax, zoffset);
                        maxRadius = Math.Max(tempMax, maxRadius);
                    }
                }
            }
           return maxRadius/2;
        }


        #region Initialize
        public Projectile(long projectileId, long firedByPlayerID, float speed, Vector3 position,Vector3 rotation, float rotationSpeed, float mass, float width, float height, float length, ProjectileType projectileType)
            : base(position,rotation, rotationSpeed, mass, width, height, length)
        {
            this.projectileID = projectileId;
            this.firedByPlayerID = firedByPlayerID;
            this.speed = speed;
            float tempradius = Math.Max(width, height);
            this.radius = Math.Max(tempradius, length);
            this.projectileType = projectileType;

        }

        public void Load(ContentManager content, String path)
        {
            base.Load(content, path);
            this.radius *= CalculateRadius() ;
            body = new Sphere(this.Position, this.radius, this.mass);
            body.Tag = this;
            body.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
        }
        #endregion

        public void Shoot()
        {
            if(body != null)
            {
                Vector3 impulse = rotation * speed;
                body.ApplyLinearImpulse(ref impulse);
            }
        }

        private  Vector3 ToEulerAngles(Quaternion q)
        {
            // Store the Euler angles in radians
            Vector3 pitchYawRoll = new Vector3();

            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            // If quaternion is normalised the unit is one, otherwise it is the correction factor
            double unit = sqx + sqy + sqz + sqw;
            double test = q.X * q.Y + q.Z * q.W;

            if (test > 0.499f * unit)
            {
                // Singularity at north pole
                pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W);  // Yaw
                pitchYawRoll.X = MathHelper.PiOver2;                // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            else if (test < -0.499f * unit)
            {
                // Singularity at south pole
                pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
                pitchYawRoll.X = -MathHelper.PiOver2;               // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }

            pitchYawRoll.Y = (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, sqx - sqy - sqz + sqw);       // Yaw
            pitchYawRoll.X = (float)Math.Asin(2 * test / unit);                                             // Pitch
            pitchYawRoll.Z = (float)Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, -sqx + sqy - sqz + sqw);      // Roll

            return pitchYawRoll;
        }
       
        public void Update()
        {
            Position = body.Position;
            rotation = ToEulerAngles(body.Orientation);
        }
        #endregion
    }
}
