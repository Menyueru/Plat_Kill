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
        
        public void FireProjectile(ProjectileType projectileType, Player playerShotted) 
        {
            Console.WriteLine("Cam Rot: ");
            
            Vector3 unitar = camera.transformedReference;
            unitar.Normalize();

            if(!((HumanPlayer)playerShotted).activeCamera.CamState.Equals(CameraState.FirstPersonCamera))
                unitar = Vector3.Multiply(unitar, -1);

            Projectile projectile = new Projectile(Interlocked.Increment(ref projectileID), playerShotted.Id, 50f,
                                                       playerShotted.Position + new Vector3(0, playerShotted.CharacterController.Body.Height, 0),
                                                       unitar, 0, 0.05f, .025f, .025f, .025f, projectileType);

            switch (projectileType)
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
                        if (p.Id == proj.FiredByPlayerID) return;
                        Player shooter=game.PlayerManager.GetPlayer(proj.FiredByPlayerID);
                        float damage = shooter.RangePower + shooter.EquippedWeapons[shooter.ActiveWeaponIndex].WeaponDamage;
                        p.Health -= (long)damage - p.Defense;
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
