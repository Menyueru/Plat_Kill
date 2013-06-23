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
        private Vector3 position;
        private float rotationSpeed;
        private Vector3 rotation;
        private Model model;
        private Matrix world;

        #endregion
        
        #region Getter-Setters

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

        public GameModel()
        {


        }

        #endregion     

        public void Load(ContentManager content, String path)
        {
            this.model = content.Load<Model>(path);
        }

        public void Draw(Matrix view, Matrix projection) 
        {
           world = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) 
                   * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateTranslation(position);

            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = projection;
                    effect.View = view;
                    effect.World = world;

                }
            }

        }
    }
}
