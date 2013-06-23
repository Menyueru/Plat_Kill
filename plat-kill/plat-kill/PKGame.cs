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
using plat_kill.Components.Camera;


namespace plat_kill
{
    public class PKGame : Microsoft.Xna.Framework.Game
    {
        CameraManager camManager;

        public PKGame()
        {
            base.IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            


            base.Initialize();
        }

        protected override void LoadContent()
        {

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
           

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

         
            base.Draw(gameTime);
        }
      
    }
}
