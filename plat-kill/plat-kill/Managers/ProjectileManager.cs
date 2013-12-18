using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
using plat_kill.Components.Camera;
using plat_kill.Events;
using plat_kill.GameModels.Players;
using plat_kill.GameModels.Projectiles;
using plat_kill.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace plat_kill.Managers
{
    public class ProjectileManager
    {
        #region Fields
        private PKGame game;
        
        private Dictionary<long, Projectile> projectiles;
        private static long projectileID;
        private DateTime lastshot;
        private TimeSpan firerate;
        
        public event EventHandler<ShotFiredArgs> ShotFired;


        public Camera camera;

        #endregion

        #region Constructor
        public ProjectileManager(PKGame game, Camera camera)
        {
            this.camera = camera;
            this.game = game;
            this.projectiles = new Dictionary<long, Projectile>();
            projectileID = 0;
            this.firerate = new TimeSpan(0, 0, 0, 0, 700);
            this.lastshot = DateTime.Now;
        }
        #endregion

        #region Methods
        public Projectile GetProjectile(long projectileID) 
        {
            if(projectiles.ContainsKey(projectileID))
            {
                return projectiles[projectileID];
            }
            return null;
        }

        public void MeleeAttack(Player playerShotted)
        {
            Ray ray = new Ray(playerShotted.Position + new Vector3(0, playerShotted.CharacterController.Body.Height/2, 0), 
                                playerShotted.World.Backward);
            RayCastResult result;
            if (game.Space.RayCast(ray, candidate => candidate != playerShotted.CharacterController.Body.CollisionInformation, out result))
            {
                var obj = result.HitObject as EntityCollidable;
                if (obj != null)
                {
                    float distance =result.HitData.T * ray.Direction.Length();
                    if (distance <= 10)
                    {
                        var person = obj.Entity.Tag as Player;
                        if (person != null)
                        {
                            float damage = playerShotted.MeleePower + playerShotted.EquippedWeapons[playerShotted.ActiveWeaponIndex].WeaponDamage;
                            float reduction = (damage * ((float)person.Defense / 100f));
                            person.Health -=(long)(damage - reduction);
                            person.LastHit = playerShotted.Id;
                        }
                    }
                }
            }
            this.lastshot = DateTime.Now;
            game.soundManager.PlaySoundFX(plat_kill.Helpers.States.SoundEffects.MeleeSwing);
        }
        
        public void FireProjectile(Player playerShotted, Vector3 projectileDir) 
        {
            ProjectileType pType;
            if (playerShotted.EquippedWeapons.Count != 0)
                pType = playerShotted.EquippedWeapons[playerShotted.ActiveWeaponIndex].ProjectileType;
            else pType = ProjectileType.Bullet;
            Projectile projectile = new Projectile(Interlocked.Increment(ref projectileID), playerShotted.Id, 50f,
                                                       playerShotted.Position + new Vector3(0, playerShotted.CharacterController.Body.Height, 0),
                                                       projectileDir, 0, 0.05f, .025f, .025f, .025f, pType);

            switch (pType)
            {
                case ProjectileType.Arrow:
                    projectile.Load(game.Content, "Models\\Objects\\AppleGreen");
                    break;
                case ProjectileType.Bullet:
                    projectile.Load(game.Content, "Models\\Objects\\AppleGreen");
                    break;
                case ProjectileType.Beam:
                    projectile.Load(game.Content, "Models\\Objects\\AppleGreen");
                    break;
            }
            projectile.Shoot();
            this.projectiles.Add(projectileID, projectile);
            this.game.Space.Add(projectile.Body);
            projectile.Body.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
            this.OnShotFired(projectile);
            this.lastshot = DateTime.Now;
            game.soundManager.PlaySoundFX(plat_kill.Helpers.States.SoundEffects.GunShot);
        }

        void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            Projectile proj = sender.Entity.Tag as Projectile;
            if (proj != null)
            {
                var otherEntityInformation = other as EntityCollidable;
                if (otherEntityInformation != null)
                {
                    Player p = otherEntityInformation.Entity.Tag as Player;
                    if (p != null)
                    {
                        if (p.Id == proj.FiredByPlayerID || p.IsDodging) return;
                        Player shooter=game.PlayerManager.GetPlayer(proj.FiredByPlayerID);
                        float damage = 0;
                        if(shooter.EquippedWeapons != null)
                            damage = shooter.RangePower + shooter.EquippedWeapons[shooter.ActiveWeaponIndex].WeaponDamage;
                        float reduction= (damage*((float)p.Defense/100f));
                        p.Health -= (long)(damage - reduction);
                        p.LastHit = shooter.Id;
                    }
                }
                proj.RemoveFromSpace = true;
            }
        }
        
        public void DrawAllBullets(Matrix View, Matrix Projection) 
        {
            foreach(Projectile bullet in projectiles.Values)
            {
                bullet.Draw(View, Projection);
            }
        }

        private void OnShotFired(Projectile shot)
        {
            EventHandler<ShotFiredArgs> shotFired = this.ShotFired;
            if (shotFired != null)
            {
                shotFired(this, new ShotFiredArgs(shot));
            }
        }

        public void UpdateAllBullets() 
        {
            List<long> keys = new List<long>(projectiles.Keys);
            foreach(long key in keys)
            {
                if (projectiles[key].RemoveFromSpace)
                {
                    game.Space.Remove(projectiles[key].Body);
                    projectiles.Remove(key);
                }
                else
                {
                    projectiles[key].Update();
                }
            }
        }
        #endregion
    }
}
