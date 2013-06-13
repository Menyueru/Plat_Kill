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

namespace plat_kill.GameModels.Players
{
    class Player : GameModel
    {
        //TODO: 
        protected int playerID;

        public int PlayerID
        {
            get { return playerID; }
            set { playerID = value; }
        }


        internal Player(GraphicsDevice device, ContentManager content) : base (device, content) 
        {

        }
    }
}
