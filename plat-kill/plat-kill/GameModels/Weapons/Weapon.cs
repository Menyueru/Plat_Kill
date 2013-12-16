using Microsoft.Xna.Framework.Graphics;
using plat_kill.GameModels.Projectiles;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Xclna.Xna.Animation;
using plat_kill.Helpers;
using plat_kill.Managers;
using plat_kill.GameModels.Players;

namespace plat_kill.GameModels.Weapons
{
    public class Weapon
    {
        #region Fields
        private WeaponType weaponType;
        private ProjectileType projectileType;

        private string name;
        private int loadedAmmo;
        private int totalAmmo;
        private float weaponDamage;
        private const int MAX_LOADED_AMMO = 20;

        private TimeSpan reloadRate;
        private DateTime lastReload;

        private TimeSpan fireRate;
        private DateTime lastShot;

        private Model weaponModel;

        #endregion

        #region Propierties
        public DateTime LastReload
        {
            get { return lastReload; }
            set { lastReload = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public TimeSpan ReloadRate
        {
            get { return reloadRate; }
            set { reloadRate = value; }
        }
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
        public TimeSpan FireRate
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

        public Weapon(ContentManager content,string name, string modelPath, WeaponType weaponType,
                      ProjectileType projectileType, float weaponDamage, float fireRate,
                      int loadedAmmo, int totalAmmo) 
        {
            this.name = name;
            this.weaponModel = LoadContent(content, modelPath);
            this.weaponType = weaponType;
            this.projectileType = projectileType;
            this.weaponDamage = weaponDamage;
            this.loadedAmmo = loadedAmmo;
            this.totalAmmo = totalAmmo;

            this.fireRate = new TimeSpan(0, 0, 0, 0, (int)fireRate);
            this.lastShot = DateTime.Now;

            this.reloadRate = new TimeSpan(0, 0, 0, 2, 0);
            this.lastReload = new DateTime();

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

            Vector3 rotAxis;
            Matrix translate;
            if(weaponType.Equals(WeaponType.Melee))
            {
                rotAxis = new Vector3(7.5f*MathHelper.PiOver4, 2.5f * MathHelper.PiOver2, 4.5f*MathHelper.PiOver2);
                translate = Matrix.CreateTranslation(-0.16f, -0.24f, 0.8f);
            }
            else
            {
                rotAxis = new Vector3(.5f * MathHelper.PiOver4, 2.5f * MathHelper.PiOver2, 2f * MathHelper.PiOver2);
                translate = Matrix.CreateTranslation(0f, -0.3f, 0);
            }
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

                    effect.World =  Matrix.CreateFromAxisAngle(rotAxis, MathHelper.Pi) * translate
                                 * Model1TransfoMatrix 
                                 * model2Transform;
                    effect.View = view;
                    effect.Projection = projection;
                }
                mesh.Draw();
            }
        }

        public void DrawOnFloor(Vector3 position, Matrix view, Matrix projection) 
        {
            Matrix world =  Matrix.CreateTranslation(position);

            foreach (ModelMesh mesh in weaponModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Projection = projection;
                    effect.View = view;
                    effect.World = world;
                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        public void ReloadWeapon(Player player) 
        {
            if(this.WeaponType.Equals(WeaponType.Range))
            {
                if (this.LoadedAmmo < MAX_LOADED_AMMO && ((LastReload.Add(ReloadRate)) < DateTime.Now))
                {
                    int amountToLoad = MAX_LOADED_AMMO - this.LoadedAmmo;
                    if (((this.TotalAmmo - amountToLoad) > 0))
                    {
                        player.IsReloading = true;
                        this.LastReload = DateTime.Now;
                        this.LoadedAmmo += amountToLoad;
                        this.TotalAmmo -= amountToLoad;
                    }
                    else if (this.TotalAmmo > 0)
                    {
                        player.IsReloading = true;
                        this.LastReload = DateTime.Now;
                        this.LoadedAmmo = this.TotalAmmo;
                        this.TotalAmmo = 0;
                    }                    
                }

            }
        }

        public void Shoot(ProjectileManager projectileManager, Player playerWhoIsFiring, Vector3 bulletDir)
        {
            if (((lastShot.Add(fireRate)) < DateTime.Now) && ((LastReload.Add(ReloadRate)) < DateTime.Now))
            {
                if (this.weaponType.Equals(WeaponType.Melee))
                {
                    if (!playerWhoIsFiring.IsShooting)
                    {
                        playerWhoIsFiring.IsShooting = true;
                        projectileManager.MeleeAttack(playerWhoIsFiring);
                    }
                }
                else
                {
                    if (this.loadedAmmo > 0 && !playerWhoIsFiring.IsDodging)
                    {
                        playerWhoIsFiring.IsShooting = true;
                        projectileManager.FireProjectile(playerWhoIsFiring, bulletDir);
                        this.LoadedAmmo -= 1;
                    }
                    else
                    {
                        ReloadWeapon(playerWhoIsFiring);
                    }

                }
                this.lastShot = DateTime.Now;
            }           
        }

        #endregion

    }
}
