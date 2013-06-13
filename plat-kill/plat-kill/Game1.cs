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
using plat_kill.Components;
using plat_kill.GameModels.Players;
using plat_kill.Managers;

namespace plat_kill
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;


        KeyboardState lastKeyboardState = new KeyboardState();
        GamePadState lastGamePadState = new GamePadState();
        MouseState lastMousState = new MouseState();
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState currentGamePadState = new GamePadState();
        MouseState currentMouseState = new MouseState();

        PlayerManager playerManager;

        HumanPlayer player;

        Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
            
            
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 853;
            graphics.PreferredBackBufferHeight = 480;

            // Create the chase camera
            camera = new Camera();

            // Set the camera offsets
            camera.DesiredPositionOffset = new Vector3(0.0f, 2000.0f, 3500.0f);
            camera.LookAtOffset = new Vector3(0.0f, 150.0f, 0.0f);

            // Set camera perspective
            camera.NearPlaneDistance = 10.0f;
            camera.FarPlaneDistance = 100000.0f;

            //TODO: Set any other camera invariants here such as field of view
        }

        protected override void Initialize()
        {
            player = new HumanPlayer(GraphicsDevice, Content);
            player.PlayerID = 0;

            base.Initialize();
            
            playerManager = new PlayerManager();
            playerManager.AddPlayer(player);

            camera.AspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Height;

            camera.UpdateCameraChaseTarget(playerManager.GetPlayer(0));
            camera.Reset();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            spriteFont = Content.Load<SpriteFont>("Fonts\\gameFont");

            player.Load("Models\\Characters\\IronMan");
        }

        protected override void Update(GameTime gameTime)
        {
            lastKeyboardState = currentKeyboardState;
            lastGamePadState = currentGamePadState;
            lastMousState = currentMouseState;

            currentKeyboardState = Keyboard.GetState();

            currentGamePadState = GamePad.GetState(PlayerIndex.One);
            currentMouseState = Mouse.GetState();

            if (currentKeyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }


            playerManager.UpdateAllPlayers(gameTime);

            camera.UpdateCameraChaseTarget(playerManager.GetPlayer(0));
            camera.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.CornflowerBlue);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            playerManager.DrawAllPlayers(camera.View, camera.Projection);
         
            base.Draw(gameTime);
        }
      
    }
}
