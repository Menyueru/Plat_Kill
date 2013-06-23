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

        #endregion

        #region Initialization

        public GameModel()
        {


        }

        #endregion     

        public void Load()
        {

        }

        public void Draw(Matrix view, Matrix projection) 
        {

        }
    }
}
