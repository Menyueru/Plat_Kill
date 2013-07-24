using System;
using plat_kill.GameModels.Projectiles;

namespace plat_kill.Events
{

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ShotFiredArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ShotFiredArgs"/> class.
        /// </summary>
        /// <param name="shot">
        /// The shot.
        /// </param>
        public ShotFiredArgs(Projectile shot)
        {
            this.Shot = shot;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Shot.
        /// </summary>
        public Projectile Shot { get; private set; }

        #endregion
    }
}