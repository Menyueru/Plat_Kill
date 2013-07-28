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
using plat_kill.Networking;
using Lidgren.Network;
using plat_kill.Networking.Messages;


namespace plat_kill
{
    public class PKGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        CameraManager camManager;

        PlayerManager playerManager;
        
        ProjectileManager projectileManager;

        private INetworkManager networkManager;
        private long localPlayerId;

        private bool IsHost
        {
            get
            {
                return this.networkManager is ServerNetworkManager;
            }
        }

        public ProjectileManager ProjectileManager
        {
            get { return projectileManager; }
            set { projectileManager = value; }
        }

        private SkyBox skyBox;
        private Terrain map;
        private Space space;

        public Space Space
        {
            get { return space; }
            set { space = value; }
        }
        

        long playerID = 0;

        public PKGame(INetworkManager networkManager)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferMultiSampling = false;
            graphics.IsFullScreen = false;
            
            this.networkManager = networkManager;
            
            base.IsMouseVisible = false;
        }

        protected override void Initialize()
        {
            this.networkManager.Connect();

            this.space = new Space();
            this.space.ForceUpdater.Gravity = new Vector3(0, -9.81f, 0);

            Camera camera = new Camera((float) graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Width);

            this.projectileManager = new ProjectileManager(this);
            this.projectileManager.ShotFired += (sender, e) => this.networkManager.SendMessage(new ShotFiredMessage(e.Shot));

            this.playerManager = new PlayerManager();
            this.playerManager.PlayerStateChanged += (sender, e) => this.networkManager.SendMessage(new UpdatePlayerStateMessage(e.Player));

            if(this.IsHost)
            {
                localPlayerId = playerID++;
                HumanPlayer player = new HumanPlayer(localPlayerId, 100, 100, 100, 100, 100, 40, 100, new Vector3(0, 10, 0), 5f / 60f, 30, 0.25f, 0.25f, 0.25f, true, this);
                player.Load(Content, "Models\\Characters\\dude");
                space.Add(player.Body);
                playerManager.AddPlayer(player);
                camera.SetTargetToChase(playerManager.GetPlayer(localPlayerId).Position, playerManager.GetPlayer(localPlayerId).Rotation,
                        playerManager.GetPlayer(localPlayerId).PlayerHeadOffset);
            }



            camManager = new CameraManager(camera, CameraState.State.ThirdPersonCamera);

            skyBox = new SkyBox(graphics.GraphicsDevice);
            map = new Terrain();        

            base.Initialize();
        }

        protected override void LoadContent()
        {
            map.LoadContent(this.Content, "Models\\Maps\\playground");
            space.Add(map.Mesh);

            skyBox.Load(this.Content, "Textures\\SkyBoxes\\BlueSky\\SkyEffect", "Textures\\SkyBoxes\\BlueSky\\SkyBoxTex");

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            space.Update();
            ProcessNetworkMessages();
            playerManager.UpdateAllPlayers(gameTime);
            projectileManager.UpdateAllBullets();

            if (playerManager.GetPlayer(localPlayerId) != null)
            {
                camManager.UpdateAllCameras(playerManager.GetPlayer(localPlayerId).Position,
                                            playerManager.GetPlayer(localPlayerId).Rotation,
                                            playerManager.GetPlayer(localPlayerId).PlayerHeadOffset,
                                            ((HumanPlayer)playerManager.GetPlayer(localPlayerId)).CameraDistance);
            }
            else {
                Exit();
            }
            
            
            if(Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        private void ProcessNetworkMessages()
        {
            NetIncomingMessage im;

            while ((im = this.networkManager.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(im.ReadString());
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)im.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                if (!this.IsHost)
                                {
                                    var message = new UpdatePlayerStateMessage(im.SenderConnection.RemoteHailMessage);
                                    localPlayerId = message.Id;
                                    HumanPlayer player = new HumanPlayer(localPlayerId, 100, 100, 100, 100, 100, 40, 100, new Vector3(0, 10, 0), 5f / 60f, 30, 0.25f, 0.25f, 0.25f,true,this);
                                    player.Load(this.Content, "Models\\Characters\\dude");
                                    space.Add(player.Body);
                                    playerManager.AddPlayer(player);
                                    camManager.ActiveCamera.SetTargetToChase(playerManager.GetPlayer(localPlayerId).Position, playerManager.GetPlayer(localPlayerId).Rotation,
                                    playerManager.GetPlayer(localPlayerId).PlayerHeadOffset);
                                    Console.WriteLine("Connected to {0}", im.SenderEndPoint);
                                }
                                else
                                {
                                    Console.WriteLine("{0} Connected", im.SenderEndPoint);
                                }

                                break;
                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine(
                                    this.IsHost ? "{0} Disconnected" : "Disconnected from {0}", im.SenderEndPoint);
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                NetOutgoingMessage hailMessage = this.networkManager.CreateMessage();
                                Player player1= new Player(playerID++, 100, 100, 100, 100, 100, 40, 100, new Vector3(0, 10, 0), 5f / 60f, 30, 0.25f, 0.25f, 0.25f,false);
                                player1.Load(Content, "Models\\Characters\\dude");
                                space.Add(player1.Body);
                                playerManager.AddPlayer(player1);
                                new UpdatePlayerStateMessage(player1).Encode(hailMessage);
                                im.SenderConnection.Approve(hailMessage);
                                break;
                        }

                        break;
                    case NetIncomingMessageType.Data:
                        var gameMessageType = (GameMessageTypes)im.ReadByte();
                        switch (gameMessageType)
                        {
                            case GameMessageTypes.UpdatePlayerState:
                                this.HandleUpdatePlayerStateMessage(im);
                                break;
                            case GameMessageTypes.ShotFired:
                                this.HandleShotFiredMessage(im);
                                break;
                        }

                        break;
                }

                this.networkManager.Recycle(im);
            }
        }

        private void HandleShotFiredMessage(NetIncomingMessage im)
        {
            var message = new ShotFiredMessage(im);

            if (message.FiredById != localPlayerId)
            {
                this.projectileManager.FireProjectile(ProjectileType.Bullet, playerManager.GetPlayer(message.FiredById));
            }
            
        }

        private void HandleUpdatePlayerStateMessage(NetIncomingMessage im)
        {
            var message = new UpdatePlayerStateMessage(im);

            var timeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(message.MessageTime));

             Player player;
            if (this.playerManager.GetPlayer(message.Id) != null)
            {
                player = this.playerManager.GetPlayer(message.Id);
            }
            else 
            {
                player = new Player(message.Id, 100, 100, 100, 100, 100, 40, 100, message.Position, 5f / 60f, 30, 0.25f, 0.25f, 0.25f,false);
                player.Load(this.Content, "Models\\Characters\\dude");
                space.Add(player.Body);    
                playerManager.AddPlayer(player);
            }

            player.CharecterState = message.CharacterState;
            player.Body.LinearVelocity = message.Velocity;                                        
            player.Position = message.Position += message.Velocity * timeDelay;
            player.Rotation = message.Rotation;

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            skyBox.Draw(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            map.Draw(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);

            playerManager.DrawAllPlayers(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            projectileManager.DrawAllBullets(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);


            base.Draw(gameTime);
        }
      
    }
}
