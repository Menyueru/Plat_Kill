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
        #region Propierties, Getters and Setters
        private Game game;

        private ContentManager content;

        private Model myModel;

        private Vector3 modelPosition;

        public Vector3 ModelPosition
        {
            get { return modelPosition; }
            set { modelPosition = value; }
        }

        private float modelRotation;

        public float ModelRotation
        {
            get { return modelRotation; ; }
            set { modelRotation = value; }
        }

        private float aspectRatio;

        public float AspectRatio
        {
            get { return aspectRatio; }
            set { aspectRatio = value; }
        }

        #endregion

        #region Constructor
        public GameModel(Game game, ContentManager content) 
        {
            this.game = game;
            this.content = content;

        }
        #endregion

        #region Methods
        public void Load(String path) 
        {
            
            myModel = content.Load<Model>(path);
            aspectRatio = game.GraphicsDevice.Viewport.AspectRatio;

        }

        public void Draw(GameTime gameTime, Vector3 cameraPosition) 
        {
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateTranslation(modelPosition);

                    effect.View = Matrix.CreateLookAt(cameraPosition,
                        Vector3.Zero, Vector3.Up);

                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,1.0f, 10000.0f);
                }

                mesh.Draw();
            }

        }
        #endregion


    }
}
