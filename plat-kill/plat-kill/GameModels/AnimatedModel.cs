﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using plat_kill.Helpers;
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
        private Vector3 movementCurrentDirection;
        private ModelAnimator modelAnimator;
        private CharacterState charecterState;

        private AnimationController currentAnimationController;
        private AnimationController previousAnimationController;
        private AnimationController tPose, rifleWalk, rifleRun, shootRifle, firingRifle, rifleJumpInPlace, greatSwordSlash, rifleIdle,
                  reloading, reload, tossGrenade, dodging, standardWalk, running, sprintingFowardRoll, falling;
        private Model model;
        
        protected float width;
        protected float height;
        protected float length;
        protected float mass;
        protected Matrix orientationMatrix;

        #endregion
        
        #region Getter-Setters 
        public AnimationController Falling
        {
            get { return falling; }
            set { falling = value; }
        }
        public Vector3 MovementCurrentDirection
        {
            get { return movementCurrentDirection; }
            set { movementCurrentDirection = value; }
        }
        public AnimationController SprintingFowardRoll
        {
            get { return sprintingFowardRoll; }
            set { sprintingFowardRoll = value; }
        }

        public AnimationController Running
        {
            get { return running; }
            set { running = value; }
        }

        public AnimationController StandardWalk
        {
            get { return standardWalk; }
            set { standardWalk = value; }
        }

        public AnimationController Dodging
        {
            get { return dodging; }
            set { dodging = value; }
        }

        public AnimationController TossGrenade
        {
            get { return tossGrenade; }
            set { tossGrenade = value; }
        }

        public AnimationController Reload
        {
            get { return reload; }
            set { reload = value; }
        }

        public AnimationController Reloading
        {
            get { return reloading; }
            set { reloading = value; }
        }

        public AnimationController RifleIdle
        {
            get { return rifleIdle; }
            set { rifleIdle = value; }
        }

        public AnimationController GreatSwordSlash
        {
            get { return greatSwordSlash; }
            set { greatSwordSlash = value; }
        }

        public AnimationController RifleJumpInPlace
        {
            get { return rifleJumpInPlace; }
            set { rifleJumpInPlace = value; }
        }

        public AnimationController FiringRifle
        {
            get { return firingRifle; }
            set { firingRifle = value; }
        }

        public AnimationController ShootRifle
        {
            get { return shootRifle; }
            set { shootRifle = value; }
        }

        public AnimationController RifleRun
        {
            get { return rifleRun; }
            set { rifleRun = value; }
        }

        public AnimationController RifleWalk
        {
            get { return rifleWalk; }
            set { rifleWalk = value; }
        }

        public AnimationController TPose
        {
            get { return tPose; }
            set { tPose = value; }
        }

        public CharacterState CharecterState
        {
            get { return charecterState; }
            set { charecterState = value; }
        }
        public AnimationController PreviousAnimationController
        {
            get { return previousAnimationController; }
            set { previousAnimationController = value; }
        }
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
            this.orientationMatrix = Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y)
                                    * Matrix.CreateRotationZ(rotation.Z);
        }

        #endregion     

        public void Load(ContentManager content, String path, GraphicsDevice graphics, Matrix view, Matrix projection)
        {
            this.model = content.Load<Model>(path);
            ModelAnimator = new ModelAnimator(model);

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect basic = (BasicEffect)effect;
                        basic.View = view;
                        basic.Projection = projection;
                    }
                    else if (effect is BasicPaletteEffect)
                    {
                        BasicPaletteEffect palette = (BasicPaletteEffect)effect;
                        palette.View = view;
                        palette.Projection = projection;
                        palette.EnableDefaultLighting();
                        palette.DirectionalLight0.Direction = new Vector3(0, 0, 1);
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, Matrix view, Matrix projection) 
        {
            Vector3 modelRotationModifier = Vector3.Zero;
            if (CharecterState == CharacterState.FiringRifle 
                || CharecterState == CharacterState.ShootRifle)
            {
               modelRotationModifier.Y = 100;
            }
            else if((CharecterState == CharacterState.RifleRun || CharecterState == CharacterState.Running) && (MovementCurrentDirection.X > 0))
            {
                modelRotationModifier.Y = 100;
            }


           orientationMatrix =  Matrix.CreateRotationX(rotation.X + modelRotationModifier.X) 
                                    * Matrix.CreateRotationY(rotation.Y + modelRotationModifier.Y)
                                    * Matrix.CreateRotationZ(rotation.Z + modelRotationModifier.Z) 
                                    * Matrix.CreateRotationY(MathHelper.Pi);
           world = orientationMatrix * Matrix.CreateTranslation(position);
           
            ModelAnimator.World = world;
           foreach (ModelMesh mesh in ModelAnimator.Model.Meshes)
           {
               foreach (Effect effect in mesh.Effects)
               {
                   effect.Parameters["View"].SetValue(view);
                   effect.Parameters["Projection"].SetValue(projection);
               }
           }

           ModelAnimator.Draw(gameTime);
           orientationMatrix = Matrix.CreateRotationX(rotation.X)
                                    * Matrix.CreateRotationY(rotation.Y)
                                    * Matrix.CreateRotationZ(rotation.Z)
                                    * Matrix.CreateRotationY(MathHelper.Pi);
           world = orientationMatrix * Matrix.CreateTranslation(position);
        }

        public void Update(GameTime gameTime) 
        {
            ModelAnimator.Update(gameTime);
            currentAnimationController.Update(gameTime);
        }

        protected void runAnimationController(ModelAnimator animator, AnimationController controller)
        {
            this.PreviousAnimationController = currentAnimationController;
            this.currentAnimationController = controller;

            foreach (BonePose p in animator.BonePoses)
            {
                p.CurrentController = this.PreviousAnimationController;
                p.CurrentBlendController = this.CurrentAnimationController;
                p.BlendFactor = 0.5f;
            }
        }

        protected void changeCharacterState(CharacterState newState) 
        {
            this.charecterState = newState;
        }
    }
}
