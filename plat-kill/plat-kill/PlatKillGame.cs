using System;
using Microsoft.Xna.Framework;
using plat_kill.GameScreens;
using plat_kill.GameScreens.ScreenComponents;
using plat_kill.GameScreens.Screens;
using Microsoft.Xna.Framework.Input;

namespace plat_kill
{
    public class PlatKillGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        ScreenFactory screenFactory;

        #endregion

        #region Constructor
        public PlatKillGame()
        {
            graphics = new GraphicsDeviceManager(this);

            TargetElapsedTime = TimeSpan.FromTicks(333333);

            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferMultiSampling = false;
            graphics.IsFullScreen = true ;

            base.IsMouseVisible = false;

            // Create the screen factory and add it to the Services
            screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            // Create the screen manager component.

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            AddInitialScreens();
        }
        #endregion

        #region Methods

        private void AddInitialScreens()
        {
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new TitleScreen(), null);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        #endregion

    }
}
