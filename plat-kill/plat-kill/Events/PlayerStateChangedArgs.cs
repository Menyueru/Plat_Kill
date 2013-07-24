using System;
using plat_kill.GameModels.Players;

namespace plat_kill.Events
{
    public class PlayerStateChangedArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStateChangedArgs"/> class.
        /// </summary>
        /// <param name="player">
        /// The player.
        /// </param>
        public PlayerStateChangedArgs(Player player)
        {
            this.Player = player;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets Player.
        /// </summary>
        public Player Player { get; private set; }

        #endregion
    }
}