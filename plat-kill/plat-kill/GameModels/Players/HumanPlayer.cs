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
        public void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            KeyboardState keyboard;
            float dt = Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (keyboard.IsKeyDown(Keys.W))
            {
                MoveForward(dt);
            }
            else if (keyboard.IsKeyDown(Keys.S))
            {
                MoveForward(-dt);
            }
            if(keyboard.IsKeyDown(Keys.D))
            {
                MoveRight(dt);
            }
            else if (keyboard.IsKeyDown(Keys.A))
            {
                MoveRight(-dt);
            }
            Move();
            if (keyboard.IsKeyDown(Keys.Space) && !Airborne)
            {
                jump();
            }

            
        }
        
        #endregion
    }
}
