﻿using Microsoft.Xna.Framework.Graphics;
using plat_kill.GameModels.Projectiles;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Xclna.Xna.Animation;
using plat_kill.Helpers;

namespace plat_kill.GameModels.Weapons
{
    public class Weapon
    {
        #region Fields
        private WeaponType weaponType;
        private ProjectileType projectileType;

        private int loadedAmmo;
        private int totalAmmo;

        private float weaponDamage;
        private float fireRate;

        private Model weaponModel;

        #endregion

        #region Propierties
        public int LoadedAmmo
        {
            get { return loadedAmmo; }
            set { loadedAmmo = value; }
        }
        public ProjectileType ProjectileType
        {
            get { return projectileType; }
            set { projectileType = value; }
        }
        public WeaponType WeaponType
        {
            get { return weaponType; }
            set { weaponType = value; }
        }
        public float FireRate
        {
            get { return fireRate; }
            set { fireRate = value; }
        }
        public float WeaponDamage
        {
            get { return weaponDamage; }
            set { weaponDamage = value; }
        }
        public int TotalAmmo
        {
            get { return totalAmmo; }
            set { totalAmmo = value; }
        }
        #endregion

        public Weapon(ContentManager content, string modelPath, WeaponType weaponType,
                      ProjectileType projectileType, float weaponDamage, float fireRate,
                      int loadedAmmo, int totalAmmo) 
        {
            this.weaponModel = LoadContent(content, modelPath);
            this.weaponType = weaponType;
            this.projectileType = projectileType;
            this.weaponDamage = weaponDamage;
            this.fireRate = fireRate;
            this.loadedAmmo = loadedAmmo;
            this.totalAmmo = totalAmmo;
        }

        #region Methods
        private Model LoadContent(ContentManager content, string modelPath) 
        {
            return content.Load<Model>(modelPath);
        }

        public void DrawOnCharacter(ModelAnimator modelAnimator, Vector3 rotation, Matrix view, Matrix projection, Vector3 position, CharacterState charState) 
        {
            Matrix Model1TransfoMatrix = modelAnimator.GetAbsoluteTransform(modelAnimator.BonePoses["vincent:RightHandIndex1"].Index);
            Matrix[] Model2TransfoMatrix = new Matrix[weaponModel.Bones.Count];

            weaponModel.CopyAbsoluteBoneTransformsTo(Model2TransfoMatrix);

            Vector3 rotAxis = new Vector3(3.5f, 2.5f * MathHelper.PiOver2, 3.5f);
            rotAxis.Normalize();


            if (charState.Equals(CharacterState.FiringRifle) || charState.Equals(CharacterState.ShootRifle))
            {
                rotation.Y += 100;
            }

            Matrix tempRotation = Matrix.CreateRotationX(rotation.X)
                                  * Matrix.CreateRotationY(rotation.Y)
                                  * Matrix.CreateRotationZ(rotation.Z);

            foreach (ModelMesh mesh in weaponModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    Matrix model2Transform = tempRotation * Matrix.CreateRotationY(MathHelper.Pi)
                                            * Matrix.CreateTranslation(position) ;

                    effect.World =  Matrix.CreateFromAxisAngle(rotAxis, MathHelper.PiOver2) 
                                 * Model1TransfoMatrix 
                                 * model2Transform;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }

        public void DrawOnFloor() 
        {
            //TODO
        }

        public void ReloadWeapon() 
        {
            //TODO
        }

        #endregion

    }
}
