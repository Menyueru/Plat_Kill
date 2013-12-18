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
using plat_kill.Events;
using plat_kill.Helpers;
using plat_kill.Networking;

namespace plat_kill.Managers
{
    public class PlayerManager
    {
        #region Propierties, Getters and Setters

        public event EventHandler<PlayerStateChangedArgs> PlayerStateChanged;

        private Dictionary<long, Player> players;

        private List<Vector3> spawnPoints;

        private int nextPoint;

        public long LocalPlayer{get;set;}

        private PKGame game;


        public List<Vector3> SpawnPoints
        {
            set { spawnPoints = value; }
        }

        public IEnumerable<Player> Players
        {
            get
            {
                return this.players.Values;
            }
        }

        #endregion

        #region Constructors
        public PlayerManager( PKGame game)
        {
            this.players = new Dictionary<long, Player>();
            this.nextPoint = 0;
            this.game = game;
        }
        #endregion

        #region Methods
        public Vector3 nextSpawnPoint()
        {
            if (nextPoint  >= spawnPoints.Count) nextPoint = 0;
            return spawnPoints[nextPoint++];
        }

        public Vector3 nextSpawnPoint(int id)
        {
            if (id >= spawnPoints.Count) return spawnPoints[0];
            return spawnPoints[id];
        }
       
        protected void OnPlayerStateChanged(Player player)
        {
            EventHandler<PlayerStateChangedArgs> playerStateChanged = this.PlayerStateChanged;
            if (playerStateChanged != null)
            {
                playerStateChanged(this, new PlayerStateChangedArgs(player));
            }
        }

        public void AddPlayer(Player player)
        {
            if (!players.ContainsKey(player.Id))
            {
                players.Add(player.Id, player);
                if (player.IsLocal)
                {
                    LocalPlayer = player.Id;
                }
            }
        }

        public Player GetPlayer(long playerID)
        {
            if (this.players.ContainsKey(playerID))
            {
                return this.players[playerID];
            }

            return null;
        }

        public long GetCurrentAmountOfPlayers()
        {
            return players.Count;
        }

        public void DrawAllPlayers(GameTime gameTime, Matrix view, Matrix projection)
        {
            foreach (Player player in this.Players)
            {
                if (!player.IsDead)
                {
                    player.Draw(gameTime, view, projection);
                    if (player.EquippedWeapons.Count > 0)
                        player.EquippedWeapons[player.ActiveWeaponIndex].DrawOnCharacter(player.ModelAnimator,
                                                                                         player.Rotation,
                                                                                         view, projection, player.Position,
                                                                                         player.CharecterState);
                }
            }
        }

        public void UpdateAllPlayers(GameTime gameTime)
        {
            if ((this.GetPlayer(this.LocalPlayer) != null))
            {
                if (this.GetPlayer(this.LocalPlayer).CharacterController.Body != null)
                    this.OnPlayerStateChanged(this.GetPlayer(this.LocalPlayer));

            }

            foreach (long key in players.Keys)
            {
                if (!players[key].IsDead)
                {
                    if (players[key].Health <= 0)
                    {
                        players[key].die();
                        game.Space.Remove(players[key].CharacterController);
                        if(this.game.NetworkManager.GetType().Equals(typeof(ServerNetworkManager)))
                        {
                            game.ScoreBoard.kill(players[key].LastHit, key);
                        }
                    }
                    if(!players[key].IsDead)
                    {
                        if (key == LocalPlayer)
                        {
                            ((HumanPlayer)players[key]).Update(gameTime);
                        }
                        else if(!(players[key] is AIPlayer))
                        {
                            players[key].Update(gameTime);
                        }
                    }
                           
                }
                else
                {
                    if (players[key].TimeOfDeath.Add(new TimeSpan(0, 0, 10)) < DateTime.Now)
                    {
                        players[key].respawn(nextSpawnPoint());
                        game.Space.Add(players[key].CharacterController);
                    }
                }
                AIPlayer test = players[key] as AIPlayer;
                if (test != null) test.Update(gameTime);
            }
        }

        #endregion

    }
}
