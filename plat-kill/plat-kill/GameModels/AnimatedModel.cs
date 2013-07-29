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
    public class AnimatedModel : GameModel
    {
        #region Fields
        private Vector3 position;
        private float rotationSpeed;
        protected Vector3 rotation;
        private Matrix world;
        private Matrix transform;

        private AnimationPlayer animationPlayer;
        private AnimationClip clip;
        private SkinningData skinningData;
        private Matrix[] boneTransforms;
        private Model model;
        private bool refresh;
        
        protected float width;
        protected float height;
        protected float length;
        protected float mass;
        protected Matrix orientationMatrix;

       

        #endregion
        
        #region Getter-Setters 
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
        public AnimationPlayer AnimationPlayer
        {
            get { return animationPlayer; }
            set { animationPlayer = value; }
        }
        public AnimationClip Clip
        {
            get { return clip; }
            set { clip = value; }
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

        public void Load(ContentManager content, String path)
        {
            this.model = content.Load<Model>(path);

            skinningData = model.Tag as SkinningData;

            if (skinningData != null)
            {
                boneTransforms = new Matrix[skinningData.BindPose.Count];

                animationPlayer = new AnimationPlayer(skinningData);

                this.clip = skinningData.AnimationClips["Take 001"];

                animationPlayer.StartClip(clip);
            }
        }

        public void Draw(Matrix view, Matrix projection) 
        {
            orientationMatrix = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y)
                                    * Matrix.CreateRotationZ(rotation.Z);
           world = transform * orientationMatrix * Matrix.CreateTranslation(position);
           Matrix[] bones = animationPlayer.GetSkinTransforms();
            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

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
            if(refresh)
            {
                animationPlayer.UpdateBoneTransforms(gameTime.ElapsedGameTime, true);
            }
            
            animationPlayer.GetBoneTransforms().CopyTo(boneTransforms, 0);
            animationPlayer.UpdateWorldTransforms(Matrix.Identity, boneTransforms);
            animationPlayer.UpdateSkinTransforms();
        }

    }
}
