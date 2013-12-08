using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.NarrowPhaseSystems.Pairs;
using Microsoft.Xna.Framework;
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

        #endregion

        #region Constructor
        public ProjectileManager(PKGame game) 
        {
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
            if ((lastshot.Add(firerate)) < DateTime.Now)
            {
                Projectile projectile = new Projectile(Interlocked.Increment(ref projectileID), playerShotted.Id, -150,
                                                       playerShotted.Position + playerShotted.World.Forward
                                                       + new Vector3(0, 6, 0),playerShotted.Rotation, 0, 0.05f, .025f, .025f, .025f, projectileType);

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
                projectile.Shoot(playerShotted.World.Forward);
                this.projectiles.Add(projectileID, projectile);

                this.game.Space.Add(projectile.Body);
                //projectile.Body.CollisionInformation.Events.InitialCollisionDetected += HandleCollision;
                this.OnShotFired(projectile);
                this.lastshot = DateTime.Now;
            }
        }

        void HandleCollision(EntityCollidable sender, Collidable other, CollidablePairHandler pair)
        {
            var otherEntityInformation = other as EntityCollidable;
            if (otherEntityInformation != null && sender!= null)
            {
                long collisionid=(long)sender.Entity.Tag;
                if (projectiles[collisionid].Colisiontime==0)
                    projectiles[collisionid].Colisiontime=DateTime.Now.Millisecond+1000;
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
            List<long> deleteids=new List<long>();
            foreach(Projectile bullet in projectiles.Values)
            {
                if (bullet.Colisiontime!=0 && bullet.Colisiontime <= DateTime.Now.Millisecond)
                {
                    deleteids.Add(bullet.ProjectileID);
                }
                bullet.Update();
            }
            /*foreach (long id in deleteids)
            {
                game.Space.Remove(projectiles[id].Body);
                projectiles.Remove(id);
            }*/
        }
        #endregion
    }
}
