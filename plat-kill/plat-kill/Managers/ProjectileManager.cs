using Microsoft.Xna.Framework;
using plat_kill.Events;
using plat_kill.GameModels.Players;
using plat_kill.GameModels.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace plat_kill.Managers
{
    public class ProjectileManager
    {
        #region Fields
        private PKGame game;
        
        private Dictionary<long, Projectile> projectiles;
        private static long projectileID;
        
        public event EventHandler<ShotFiredArgs> ShotFired;

        #endregion

        #region Constructor
        public ProjectileManager(PKGame game) 
        {
            this.game = game;
            this.projectiles = new Dictionary<long, Projectile>();
            projectileID = 0;
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
            Projectile projectile = new Projectile(Interlocked.Increment(ref projectileID), playerShotted.Id, -50,
                                                   playerShotted.Position + playerShotted.Body.OrientationMatrix.Forward
                                                   + new Vector3(0, 5, 0), 0, 0.1f, .25f, .25f, .25f, projectileType);
            switch (projectileType)
            {
                case ProjectileType.Arrow:
                    projectile.Load(game.Content, "Models\\Objects\\bullet");
                    break;
                case ProjectileType.Bullet:
                    projectile.Load(game.Content, "Models\\Objects\\bullet");
                    break;
                case ProjectileType.Beam:
                    projectile.Load(game.Content, "Models\\Objects\\bullet");
                    break;
            }
            projectile.Shoot(playerShotted.World.Forward);
            this.projectiles.Add(projectileID, projectile);  
            
            this.game.Space.Add(projectile.Body);
              
            this.OnShotFired(projectile);
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
            foreach(Projectile bullet in projectiles.Values)
            {
                bullet.Update();
            }
        }
        #endregion
    }
}
