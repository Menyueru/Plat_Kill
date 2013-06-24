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
using plat_kill.Managers;
using plat_kill.GameModels.Players;
using plat_kill.Helpers;


namespace plat_kill
{
    public class PKGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        CameraManager camManager;
        PlayerManager playerManager;

        long playerID = 0;

        public PKGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            base.IsMouseVisible = true;
        }

        protected override void Initialize()
        {

            
            Camera camera = new Camera((float) graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Width);
            camManager = new CameraManager(camera, CameraState.State.ThirdPersonCamera);

            HumanPlayer player = new HumanPlayer(playerID++, 100, 100, 100, 100, 100, 25, 30, Vector3.Zero, 1f / 60f, 30);
            player.Load(this.Content, "Models\\PlayerMarine");
        
            playerManager = new PlayerManager();
            playerManager.AddPlayer(player);


            base.Initialize();
        }

        protected override void LoadContent()
        {
            

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            playerManager.UpdateAllPlayers(gameTime);
            
            camManager.UpdateAllCameras(playerManager.GetPlayer(0).Position,
                                        playerManager.GetPlayer(0).Rotation,
                                        playerManager.GetPlayer(0).PlayerHeadOffset, ((HumanPlayer)playerManager.GetPlayer(0)).CameraDistance);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            playerManager.DrawAllPlayers(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            
            base.Draw(gameTime);
        }
      
    }
}
