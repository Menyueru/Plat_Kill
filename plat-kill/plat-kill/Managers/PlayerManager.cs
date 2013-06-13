﻿using System;
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

        private Dictionary<int, Player> players = new Dictionary<int, Player>();

        public IEnumerable<Player> Players
        {
            get
            {
                return this.players.Values;
            }
        }

        #endregion
        
        #region Constructors
        public PlayerManager() { }
        #endregion

        #region Methods
        public void AddPlayer(Player player) 
        {
            players.Add(player.PlayerID, player);
        }

        public Player GetPlayer(int playerID) 
        {
            if (this.players.ContainsKey(playerID)) 
            {
                return this.players[playerID];
            }

            return null;
        }


        public void DrawAllPlayers(Matrix view, Matrix projection) 
        {
            foreach(Player player in this.Players)
            {
                player.Draw(view, projection);
            }
        }

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
