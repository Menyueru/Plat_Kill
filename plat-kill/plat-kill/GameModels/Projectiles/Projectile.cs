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

        
        #endregion

        #region Getter-Setter
        
        public int Colisiontime
        {
            get { return colisiontime; }
            set { colisiontime = value; }
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
        public Projectile(long projectileId, long firedByPlayerID, float speed, Vector3 position, float rotationSpeed, float mass, float width, float height, float length, ProjectileType projectileType)
            : base(position, rotationSpeed, mass, width, height, length)
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
            body.Tag = this.projectileID;
            body.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
        }
        #endregion

        public void Shoot(Vector3 Forward)
        {
            Vector3 impulse = Forward * speed;
            body.ApplyLinearImpulse(ref impulse);
        }

        public void Update()
        {
            Position = body.Position;
        }
        #endregion
    }
}
