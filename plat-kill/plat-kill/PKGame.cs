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
using plat_kill.GameModels.Weapons;
using plat_kill.Components;

namespace plat_kill
{
    public class PKGame : Game
    {
        #region Field

        private GraphicsDeviceManager graphics;
        private SpriteFont font;
        private SpriteBatch spriteBatch;

        private Texture2D sexyTexture;
        private Texture2D staminaTex;
        private Texture2D healthTex;
        private Texture2D backBar;
        private Texture2D infinity;

        private CameraManager camManager;
        private PlayerManager playerManager;
        private ProjectileManager projectileManager;
        private IGameManager gameManager;
        private INetworkManager networkManager;

        private SkyBox skyBox;
        private Terrain map;
        private Space space;

        private long localPlayerId;

        #endregion

        #region Propierties

        private bool IsHost
        {
            get
            {
                return this.networkManager is ServerNetworkManager;
            }
        }
        public PlayerManager PlayerManager
        {
            get { return playerManager; }
            set { playerManager = value; }
        }
        public ProjectileManager ProjectileManager
        {
            get { return projectileManager; }
            set { projectileManager = value; }
        }
        public Space Space
        {
            get { return space; }
            set { space = value; }
        }
        
        #endregion

        #region Constructor
        public PKGame(INetworkManager networkManager,IGameManager gameManager)
        {

            graphics = new GraphicsDeviceManager(this);

            TargetElapsedTime = TimeSpan.FromTicks(333333);

            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.PreferMultiSampling = false;
            graphics.IsFullScreen = true;

            this.gameManager = gameManager;
            this.networkManager = networkManager;

        }
        #endregion

        #region Methods
        protected override void Initialize()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);

            this.space = new Space();
            this.space.ForceUpdater.Gravity = new Vector3(0, -166.77f, 0);

            skyBox = new SkyBox(graphics.GraphicsDevice);

            map = new Terrain("Content\\Scenes\\Test.scn",this);

            this.networkManager.Connect();

            Camera camera = new Camera((float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Width);

            this.projectileManager = new ProjectileManager(this);
            this.projectileManager.ShotFired += (sender, e) => this.networkManager.SendMessage(new ShotFiredMessage(e.Shot));

            this.playerManager = new PlayerManager();
            this.playerManager.PlayerStateChanged += (sender, e) => this.networkManager.SendMessage(new UpdatePlayerStateMessage(e.Player));
            this.camManager = new CameraManager(camera, CameraState.State.ThirdPersonCamera);
            base.Initialize(); 

            if (this.IsHost)
            {
                localPlayerId = playerManager.GetCurrentAmountOfPlayers();
                HumanPlayer player = new HumanPlayer(localPlayerId, 100, 200, 100, 100, 100, 30, 100, playerManager.nextSpawnPoint(), 5f / 60f, 50, 0.15f, 0.15f, 0.15f, true, this);
                player.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                player.addWeapon(new Weapon(Content, "Models\\Objects\\M4A1", WeaponType.Range, ProjectileType.Bullet, 0f,0f,5,100));

                playerManager.AddPlayer(player);
                Vector3 chase = playerManager.GetPlayer(localPlayerId).Position;
                chase.Y = playerManager.GetPlayer(localPlayerId).CharacterController.Body.Height / 2;
                camera.SetTargetToChase(chase, playerManager.GetPlayer(localPlayerId).Rotation,
                        playerManager.GetPlayer(localPlayerId).PlayerHeadOffset);
            }

        }

        protected override void LoadContent()
        {
            map.LoadContent(Content);
            map.AddToSpace(space);

            skyBox.Load(this.Content, "Textures\\SkyBoxes\\BlueSky\\SkyEffect", "Textures\\SkyBoxes\\BlueSky\\SkyBoxTex");
            font = Content.Load<SpriteFont>("Fonts\\gameFont");

            sexyTexture = Content.Load<Texture2D>("Textures\\lolbar");
            staminaTex = Content.Load<Texture2D>("Textures\\yellowGradient");
            healthTex = Content.Load<Texture2D>("Textures\\greenGradient");
            backBar = Content.Load<Texture2D>("Textures\\rock");
            infinity = Content.Load<Texture2D>("Textures\\infinity");

            gameManager.Init(this);
        }

        protected override void Update(GameTime gameTime)
        {
            space.Update();
            ProcessNetworkMessages();
            gameManager.Update();

            playerManager.UpdateAllPlayers(gameTime);
            projectileManager.UpdateAllBullets();

            if (gameManager.GameOver())
            {
                this.Exit();
            }

            if (playerManager.GetPlayer(localPlayerId) != null)
            {
                Vector3 chase = playerManager.GetPlayer(localPlayerId).Position;
                chase.Y = chase.Y + playerManager.GetPlayer(localPlayerId).CharacterController.Body.Height / 2;
                camManager.UpdateAllCameras(chase,
                                            ((HumanPlayer)playerManager.GetPlayer(localPlayerId)).CameraRotation,
                                            playerManager.GetPlayer(localPlayerId).PlayerHeadOffset,
                                            ((HumanPlayer)playerManager.GetPlayer(localPlayerId)).CameraDistance);
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            skyBox.Draw(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            map.Draw(graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            playerManager.DrawAllPlayers(gameTime, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            projectileManager.DrawAllBullets(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);

            DrawUIComponents();

            base.Draw(gameTime);

        }

        private void DrawUIComponents()
        {
            spriteBatch.Begin();

            float totalBarWidth = 300;
            float totalBarHeigth = 15;

            float staminaWidth = (totalBarWidth / playerManager.GetPlayer(localPlayerId).MaxStamina) * playerManager.GetPlayer(localPlayerId).Stamina;
            float healthWidth = (totalBarWidth / playerManager.GetPlayer(localPlayerId).MaxHealth) * playerManager.GetPlayer(localPlayerId).Health;

            Rectangle sexyBackground = new Rectangle((GraphicsDevice.Viewport.Width / 2) - 200, GraphicsDevice.Viewport.Height - 100, 400, 100);
            Rectangle staminaRec = new Rectangle((GraphicsDevice.Viewport.Width / 2) - 150, GraphicsDevice.Viewport.Height - 20, (int)staminaWidth, (int)totalBarHeigth);
            Rectangle healthRec = new Rectangle((GraphicsDevice.Viewport.Width / 2) - 150, GraphicsDevice.Viewport.Height - 35, (int)healthWidth, (int)totalBarHeigth);
            Rectangle backStaminaRec = new Rectangle((GraphicsDevice.Viewport.Width / 2) - 150, GraphicsDevice.Viewport.Height - 20, (int)totalBarWidth, (int)totalBarHeigth);
            Rectangle backHealthRec = new Rectangle((GraphicsDevice.Viewport.Width / 2) - 150, GraphicsDevice.Viewport.Height - 35, (int)totalBarWidth, (int)totalBarHeigth);

            spriteBatch.Draw(sexyTexture, sexyBackground, Color.White);
            spriteBatch.Draw(backBar, backStaminaRec, Color.White);
            spriteBatch.Draw(backBar, backHealthRec, Color.White);
            spriteBatch.Draw(healthTex, healthRec, Color.White);
            spriteBatch.Draw(staminaTex, staminaRec, Color.White);
            
            if (playerManager.GetPlayer(localPlayerId).EquippedWeapons[playerManager.GetPlayer(localPlayerId).ActiveWeaponIndex].WeaponType.Equals(WeaponType.Range))
            {
                spriteBatch.DrawString(font, "Ammo on Weapon : " + playerManager.GetPlayer(localPlayerId).EquippedWeapons[playerManager.GetPlayer(localPlayerId).ActiveWeaponIndex].LoadedAmmo, new Vector2((GraphicsDevice.Viewport.Width / 2) - 145, (GraphicsDevice.Viewport.Height - 80)), Color.DarkSlateBlue);
                spriteBatch.DrawString(font, "Ammo on Inventory : " + playerManager.GetPlayer(localPlayerId).EquippedWeapons[playerManager.GetPlayer(localPlayerId).ActiveWeaponIndex].TotalAmmo, new Vector2((GraphicsDevice.Viewport.Width / 2) - 145, (GraphicsDevice.Viewport.Height - 65)), Color.DarkSlateBlue);
            }
            else if (playerManager.GetPlayer(localPlayerId).EquippedWeapons[playerManager.GetPlayer(localPlayerId).ActiveWeaponIndex].WeaponType.Equals(WeaponType.Melee))
            {
                Rectangle infinityRect = new Rectangle((GraphicsDevice.Viewport.Width / 2) - 130, (GraphicsDevice.Viewport.Height - 80), 90, 40);
                spriteBatch.Draw(infinity, infinityRect, Color.White);
            }

            spriteBatch.End();
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
                                    HumanPlayer player = new HumanPlayer(localPlayerId, 100, 100, 100, 100, 100, 30, 100, new Vector3(0, 50, 0), 5f / 60f, 30, 0.15f, 0.15f, 0.15f, true, this);
                                    player.Load(this.Content, "Models\\Characters\\dude", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
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
                                Player player1 = new Player(playerManager.GetCurrentAmountOfPlayers(), 100, 100, 100, 100, 100, 30, 100, new Vector3(0, 50, 0), 5f / 60f, 30, 0.15f, 0.15f, 0.15f, false);
                                player1.Load(this.Content, "Models\\Characters\\dude", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
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
                player = new Player(message.Id, 100, 100, 100, 100, 100, 30, 100, message.Position, 5f / 60f, 30, 0.15f, 0.15f, 0.15f, false);
                player.Load(this.Content, "Models\\Characters\\dude", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                playerManager.AddPlayer(player);
            }

            player.CharecterState = message.CharacterState;
            player.CharacterController.Body.LinearVelocity = message.Velocity;
            player.Position = message.Position += message.Velocity;// *timeDelay;
            player.Rotation = message.Rotation;

        }
               
        #endregion

    }
}
