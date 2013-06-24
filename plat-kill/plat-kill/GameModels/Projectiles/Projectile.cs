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
    public class Projectile : GameModel
    {
        #region Properties
        private Sphere body;
        private long projectileID;
        private float radius;
        private float speed;

        #endregion
        #region Getter-Setter
        public long ProjectileID
        {
            get { return projectileID; }
            set { projectileID = value; }
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
        #region Initialize
        public Projectile(long projectileId, float speed, Vector3 position, float rotationSpeed, float mass, float width, float height, float length)
            : base(position, rotationSpeed, mass, width, height, length)
        {
            this.projectileID = projectileId;
            this.speed = speed;
            float tempradius = Math.Max(width, height);
            this.radius = Math.Max(tempradius, length) / 2;
        }

        public void LoadContent(ContentManager content, String path)
        {
            base.Load(content, path);
            body = new Sphere(this.Position, this.radius, this.mass);
            body.Tag = this.Model;
            body.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
        }
        #endregion

        public void Shoot(Vector3 Forward)
        {
            Vector3 impulse = Forward * speed;
            body.ApplyLinearImpulse(ref impulse);
            Console.WriteLine(body.LinearMomentum);
        }

        public void Update()
        {
            Position = body.Position;

        }
        #endregion
    }
}
