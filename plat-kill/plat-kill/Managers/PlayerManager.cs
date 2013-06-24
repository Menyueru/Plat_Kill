using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using plat_kill.GameModels;
using plat_kill.GameModels.Players;

namespace plat_kill.Managers
{
    class PlayerManager
    {
        #region Propierties, Getters and Setters

        private Dictionary<long, Player> players;

        public IEnumerable<Player> Players
        {
            get
            {
                return this.players.Values;
            }
        }

        #endregion
        
        #region Constructors
        public PlayerManager()
        {
            this.players = new Dictionary<long, Player>();
        }
        #endregion

        #region Methods
        public void AddPlayer(Player player) 
        {
            players.Add(player.Id, player);
        }

        public Player GetPlayer(long playerID) 
        {
            if (this.players.ContainsKey(playerID)) 
            {
                return this.players[playerID];
            }

            return null;
        }


        /*public void DrawAllPlayers(Matrix view, Matrix projection) 
        {
            foreach(Player player in this.Players)
            {
                player.Draw(view, projection);
            }
        }*/

        public void UpdateAllPlayers(GameTime gameTime) 
        {
            foreach (HumanPlayer player in this.Players)
            {
                player.Update(gameTime);
            }
        }

        #endregion

    }
}
