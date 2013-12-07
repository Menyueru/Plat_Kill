using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.GameModels
{
    public class StaticModel : GameModel
    {
        #region Fields
        private Vector3 position;
        private float rotationSpeed;
        protected Vector3 rotation;
        private Matrix world;
        private Matrix transform;

        private Model model;
        
        protected float width;
        protected float height;
        protected float length;
        protected float mass;

        #endregion
        
        #region Getter-Setters

        public Matrix Transform
        {
            get { return transform; }
            set { transform = value; }
        }

        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }

        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        public float Height
        {
            get { return height; }
            set { height = value; }
        }

        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        public Vector3 Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Model Model
        {
            get { return model; }
            set { model = value; }
        }

        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }

        #endregion

        #region Initialization

        public StaticModel(Vector3 position,Vector3 rotation, float rotationSpeed, float mass, float width, float height, float length)
        {
            this.position = position;
            this.rotationSpeed = rotationSpeed;
            this.rotation = rotation;
            this.mass = mass;
            this.height = height;
            this.length = length;
            this.width = width;
            this.transform = Matrix.CreateScale(width,height,length);
        }

        #endregion     

        public void Load(ContentManager content, String path)
        {
            this.model = content.Load<Model>(path);
        }

        public void Draw(Matrix view, Matrix projection) 
        {
           world = transform * Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) 
                   * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateTranslation(position);
            
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {   
                    effect.Projection = projection;
                    effect.View = view;
                    effect.World = world;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }

        public void Update(GameTime gameTime) 
        {
        }
    }
}
