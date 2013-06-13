using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace plat_kill.GameModels
{
    class GameModel
    {
        
        #region Fields

        private Model myModel;

        private ContentManager content;

        protected const float MinimumAltitude = 350.0f;

        private GraphicsDevice graphicsDevice;

        protected Vector3 position;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        private Vector3 direction;

        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        private Vector3 up;

        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }

        private Vector3 right;

        public Vector3 Right
        {
            get { return right; }
            set { right = value; }
        }
        

        private const float rotationRate = 1.5f;

        protected float RotationRate
        {
            get { return rotationRate; }
        } 

        private const float mass = 1.0f;

        protected float Mass
        {
            get { return mass; }
        } 

        protected const float ThrustForce = 24000.0f;

        protected const float DragFactor = 0.97f;

        private Vector3 velocity;

        protected Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        protected Matrix world;

        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }


        #endregion

        #region Initialization

        public GameModel(GraphicsDevice device, ContentManager content)
        {
            this.graphicsDevice = device;
            this.content = content;

            this.Position = new Vector3(0, MinimumAltitude, 0);
            this.Direction = Vector3.Forward;
            this.Up = Vector3.Up;
            this.right = Vector3.Right;
            this.Velocity = Vector3.Zero;

        }

        #endregion     

        public void Load(String path)
        {
            this.myModel = content.Load<Model>(path);
        }

        public void Draw(Matrix view, Matrix projection) 
        {
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * World;

                    // Use the matrices provided by the camera
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }
    }
}
