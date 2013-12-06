using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xclna.Xna.Animation;


namespace plat_kill.GameModels
{
    public class AnimatedModel
    {
        #region Fields
        private Vector3 position;
        private float rotationSpeed;
        protected Vector3 rotation;
        private Matrix world;
        private Matrix transform;

        private ModelAnimator modelAnimator;
        private AnimationController currentAnimationController;

        private Model model;
        private bool refresh;
        
        protected float width;
        protected float height;
        protected float length;
        protected float mass;
        protected Matrix orientationMatrix;

        #endregion
        
        #region Getter-Setters 
        public AnimationController CurrentAnimationController
        {
            get { return currentAnimationController; }
            set { currentAnimationController = value; }
        }
        public ModelAnimator ModelAnimator
        {
            get { return modelAnimator; }
            set { modelAnimator = value; }
        }

        protected Matrix OrientationMatrix
        {
            get { return orientationMatrix; }
            set { orientationMatrix = value; }
        }

        public bool Refresh
        {
            get { return refresh; }
            set { refresh = value; }
        }

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

        public AnimatedModel(Vector3 position, float rotationSpeed, float mass, float width, float height, float length)
        {
            this.position = position;
            this.rotationSpeed = rotationSpeed;
            this.rotation = new Vector3(0,0.25f,0); ;
            this.mass = mass;
            this.height = height;
            this.length = length;
            this.width = width;
            this.transform = Matrix.CreateScale(width,height,length);
            this.refresh = false;
            this.orientationMatrix = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y)
                                    * Matrix.CreateRotationZ(rotation.Z);
        }

        #endregion     

        public void Load(ContentManager content, String path, GraphicsDevice graphics, Matrix view, Matrix projection)
        {
            this.model = content.Load<Model>(path);

            Viewport port = graphics.Viewport;

            Matrix tempView = Matrix.CreateLookAt(new Vector3(0, 15, -20), Vector3.Zero, Vector3.Up);
            Matrix tempProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)port.Width / port.Height, .1f, 100000f);
            Effect myEffect = content.Load<Effect>("Effects\\skinFX");

            // Replace the old effects with your custom shader
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    BasicEffect oldEffect = (BasicEffect)part.Effect;
                    Effect newEffect = myEffect.Clone();
                    newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);

                    newEffect.Parameters["LightColor"].SetValue(new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
                    newEffect.Parameters["AmbientLightColor"].SetValue(new Vector4(1.25f, 1.25f, 1.25f, 1.0f));
                    newEffect.Parameters["Shininess"].SetValue(0.6f);
                    newEffect.Parameters["SpecularPower"].SetValue(0.4f);

                    newEffect.Parameters["View"].SetValue(tempView);
                    newEffect.Parameters["Projection"].SetValue(tempProjection);

                    part.Effect = newEffect;
                    oldEffect.Dispose();
                }
            }

            ModelAnimator = new ModelAnimator(model);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect basic = (BasicEffect)effect;
                        basic.View = tempView;
                        basic.Projection = tempProjection;
                    }
                    else if (effect is BasicPaletteEffect)
                    {
                        BasicPaletteEffect palette = (BasicPaletteEffect)effect;
                        palette.View = tempView;
                        palette.Projection = tempProjection;
                        palette.EnableDefaultLighting();
                        palette.DirectionalLight0.Direction = new Vector3(0, 0, 1);
                    }
                }
            }

        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection) 
        {
           orientationMatrix = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y)
                                    * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateRotationY(MathHelper.Pi);
           world = transform * orientationMatrix * Matrix.CreateTranslation(position- (new Vector3(0,3,0)));

           foreach (ModelMesh mesh in ModelAnimator.Model.Meshes)
           {
               foreach (Effect effect in mesh.Effects)
               {
                   //effect.Parameters["View"].SetValue(view);
                   effect.Parameters["Projection"].SetValue(projection);
                   //effect.Parameters["World"].SetValue(world);
               }
           }

           ModelAnimator.Draw(gameTime);
        }

        public void Update(GameTime gameTime) 
        {
            ModelAnimator.Update(gameTime);
            currentAnimationController.Update(gameTime);
        }

        protected void runAnimationController(ModelAnimator animator, AnimationController controller)
        {
            this.currentAnimationController = controller;
            foreach (BonePose p in animator.BonePoses)
            {
                p.CurrentController = controller;
                p.CurrentBlendController = null;
            }
        }

    }
}
