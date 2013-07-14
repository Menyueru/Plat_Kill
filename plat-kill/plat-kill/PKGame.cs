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
using plat_kill.GameModels;
using BEPUphysics;
using plat_kill.GameModels.Projectiles;


namespace plat_kill
{
    public class PKGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        CameraManager camManager;

        PlayerManager playerManager;
        ProjectileManager projectileManager;

        public ProjectileManager ProjectileManager
        {
            get { return projectileManager; }
            set { projectileManager = value; }
        }

        Terrain map;
        public Space space;

        long playerID = 0;

        public PKGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferMultiSampling = false;
            graphics.IsFullScreen = true;
            
            base.IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            space = new Space();
            space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);
            Camera camera = new Camera((float) graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Width);
            camManager = new CameraManager(camera, CameraState.State.ThirdPersonCamera);

            projectileManager = new ProjectileManager();

            HumanPlayer player = new HumanPlayer(playerID++, 100, 100, 100, 100, 100, 40, 100, new Vector3(0,10,0), 5f / 60f, 30,0.25f,0.25f,0.25f,this);

            player.Load(this.Content, "Models\\PlayerMarine");
            space.Add(player.Body);
        
            playerManager = new PlayerManager();
            playerManager.AddPlayer(player);


            map = new Terrain();        

            base.Initialize();
        }

        protected override void LoadContent()
        {
            map.LoadContent(this.Content, "Models\\Maps\\playground");
            space.Add(map.Mesh);
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            space.Update();
            playerManager.UpdateAllPlayers(gameTime);
            
            camManager.UpdateAllCameras(playerManager.GetPlayer(0).Position,
                                        playerManager.GetPlayer(0).Rotation,
                                        playerManager.GetPlayer(0).PlayerHeadOffset,
                                        ((HumanPlayer)playerManager.GetPlayer(0)).CameraDistance);

            if(Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            playerManager.DrawAllPlayers(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            projectileManager.DrawAllBullets(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            projectileManager.UpdateAllBullets();
            map.Draw(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            base.Draw(gameTime);
        }
      
    }
}
