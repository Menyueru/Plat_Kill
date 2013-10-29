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
using BEPUphysics.Settings;
using plat_kill.GameModels.Projectiles;
using plat_kill.Networking;
using Lidgren.Network;
using plat_kill.Networking.Messages;
using plat_kill.GameScreens.ScreenComponents;


namespace plat_kill
{
    public class PKGame : GameScreen
    {
        #region Field and Propierties
        public GraphicsDevice graphicsDevice { get; set; }
        public ContentManager Content { get; set; }

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

        #endregion

        #region Constructor
        public PKGame(INetworkManager networkManager, ContentManager Content, GraphicsDevice graphicsDevice)
        {
            this.Content = Content;
            this.graphicsDevice = graphicsDevice;
            
            
            this.networkManager = networkManager;

        }
        #endregion 

        #region Methods
        public void Initialize()
        {
            this.space = new Space();
            this.space.ForceUpdater.Gravity = new Vector3(0, -166.77f, 0);

            skyBox = new SkyBox(graphicsDevice);

            map = new Terrain("Models//Maps//heightmap", new String[] { "Textures//sand", "Textures//grass", "Textures//rock", "Textures//rock2" });
            
            this.networkManager.Connect();
            
            Camera camera = new Camera((float) graphicsDevice.Viewport.Width / (float)graphicsDevice.Viewport.Width);

            this.projectileManager = new ProjectileManager(this);
            this.projectileManager.ShotFired += (sender, e) => this.networkManager.SendMessage(new ShotFiredMessage(e.Shot));

            this.playerManager = new PlayerManager();
            this.playerManager.PlayerStateChanged += (sender, e) => this.networkManager.SendMessage(new UpdatePlayerStateMessage(e.Player));

            if(this.IsHost)
            {
                localPlayerId = playerID++;
                HumanPlayer player = new HumanPlayer(localPlayerId, 100, 100, 100, 100, 100, 30, 100, new Vector3(0, 50, 0), 5f / 60f, 50, 0.15f, 0.15f, 0.15f, true, this);
                player.Load(Content, "Models\\Characters\\dude", space);
                //space.Add(player.Body);

                //player.Body.AngularVelocity = new Vector3();
                //player.Body.LinearVelocity = new Vector3();

                playerManager.AddPlayer(player);
                Vector3 chase = playerManager.GetPlayer(localPlayerId).Position;
                chase.Y = playerManager.GetPlayer(localPlayerId).CharacterController.Body.Height / 2;
                camera.SetTargetToChase(chase, playerManager.GetPlayer(localPlayerId).Rotation,
                        playerManager.GetPlayer(localPlayerId).PlayerHeadOffset);
            }
            camManager = new CameraManager(camera, CameraState.State.ThirdPersonCamera);

        }

        public void LoadContent()
        {
            map.LoadContent(Content);
            space.Add(map.Mesh);

            skyBox.Load(this.Content, "Textures\\SkyBoxes\\BlueSky\\SkyEffect", "Textures\\SkyBoxes\\BlueSky\\SkyBoxTex");
        }

        public void Update(GameTime gameTime)
        {
            space.Update();
            ProcessNetworkMessages();
            playerManager.UpdateAllPlayers(gameTime);
            projectileManager.UpdateAllBullets();

            if (playerManager.GetPlayer(localPlayerId) != null)
            {
                Vector3 chase = playerManager.GetPlayer(localPlayerId).Position;
                chase.Y = chase.Y+playerManager.GetPlayer(localPlayerId).CharacterController.Body.Height / 2;
                camManager.UpdateAllCameras(chase,
                                            playerManager.GetPlayer(localPlayerId).Rotation,
                                            playerManager.GetPlayer(localPlayerId).PlayerHeadOffset,
                                            ((HumanPlayer)playerManager.GetPlayer(localPlayerId)).CameraDistance);
            }

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
                                    HumanPlayer player = new HumanPlayer(localPlayerId, 100, 100, 100, 100, 100, 30, 100, new Vector3(0, 50, 0), 5f / 60f, 30, 0.15f, 0.15f, 0.15f,true,this);
                                    player.Load(this.Content, "Models\\Characters\\dude",space);
                                    playerManager.AddPlayer(player);
                                    Vector3 chase = playerManager.GetPlayer(localPlayerId).Position;
                                    chase.Y = playerManager.GetPlayer(localPlayerId).CharacterController.Body.Height / 2;
                                    camManager.ActiveCamera.SetTargetToChase(chase, playerManager.GetPlayer(localPlayerId).Rotation,
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
                                Player player1= new Player(playerID++, 100, 100, 100, 100, 100, 30, 100, new Vector3(0, 50, 0), 5f / 60f, 30, 0.15f, 0.15f, 0.15f,false);
                                player1.Load(Content, "Models\\Characters\\dude",space);
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
                player = new Player(message.Id, 100, 100, 100, 100, 100, 30, 100, message.Position, 5f / 60f, 30, 0.15f, 0.15f, 0.15f,false);
                player.Load(this.Content, "Models\\Characters\\dude",space); 
                playerManager.AddPlayer(player);
            }

            player.CharecterState = message.CharacterState;
            player.CharacterController.Body.LinearVelocity = message.Velocity;
            player.Position = message.Position += message.Velocity;// *timeDelay;
            player.Rotation = message.Rotation;

        }

        public void Draw(GameTime gameTime)
        {
            skyBox.Draw(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);

            playerManager.DrawAllPlayers(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            projectileManager.DrawAllBullets(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);

            map.Draw(graphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);

        }
        #endregion

    }
}
