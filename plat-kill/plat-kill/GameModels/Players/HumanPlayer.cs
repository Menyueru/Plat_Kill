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
    class HumanPlayer : Player
    {
        #region Property
        private KeyboardState lastKeyboard;
        private MouseState lastMouse;
        #endregion
        #region Method
        public void Update(GameTime gameTime, KeyboardState keyboard, MouseState mouse)
        {

            base.Update(gameTime);
        }
        
        #endregion
    }
}
