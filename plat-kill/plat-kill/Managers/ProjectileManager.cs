using Microsoft.Xna.Framework;
using plat_kill.GameModels.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Managers
{
    public class ProjectileManager
    {
        #region Fields
        private Dictionary<long, Projectile> projectiles;

        #endregion

        #region Constructor
        public ProjectileManager() 
        {
            this.projectiles = new Dictionary<long, Projectile>();
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
        public void AddProjectile(Projectile projectile) 
        {
            projectiles.Add(projectile.ProjectileID, projectile);
        }


        public void DrawAllBullets(Matrix View, Matrix Projection) 
        {
            foreach(Projectile bullet in projectiles.Values)
            {
                bullet.Draw(View, Projection);
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
