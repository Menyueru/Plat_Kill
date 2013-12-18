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
using plat_kill.GameModels.Players.Helpers.AI;
using plat_kill.Helpers.States;

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
        private WeaponManager weaponManager;
        private GameConfiguration gameConfiguration;

        private SkyBox skyBox;
        private Terrain map;
        private Space space;

        private World place;

        private long localPlayerId;

        public ScoreBoard ScoreBoard;

        #endregion

        #region Propierties
        public INetworkManager NetworkManager
        {
            get { return networkManager; }
            set { networkManager = value; }
        }

        private bool IsHost
        {
            get
            {
                if (this.NetworkManager == null)
                {
                    return true;
                }
                else
                {
                    return this.NetworkManager is ServerNetworkManager;
                }
            }
        }

        public PlayerManager PlayerManager
        {
            get { return playerManager; }
            set { playerManager = value; }
        }
        public WeaponManager WeaponManager
        {
            get { return weaponManager; }
            set { weaponManager = value; }
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

        public World Place
        {
            get { return place; }
            set { place = value; }
        }
        #endregion

        #region Constructor
        public PKGame(GameConfiguration gameConfiguration)
        {
            this.gameConfiguration = gameConfiguration;

            graphics = new GraphicsDeviceManager(this);

            TargetElapsedTime = TimeSpan.FromTicks(333333);

            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = gameConfiguration.ResolutionWidth;
            graphics.PreferredBackBufferHeight = gameConfiguration.ResolutionHeigth;
            graphics.IsFullScreen = gameConfiguration.IsFullScreen;
            graphics.PreferMultiSampling = false;

            this.gameManager = gameConfiguration.GameManager;

            this.networkManager = gameConfiguration.NetworkManager;

        }
        #endregion

        #region Methods            
        protected override void Initialize()
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice);
            
            this.space = new Space();
            this.space.ForceUpdater.Gravity = new Vector3(0, -166.77f, 0);

            this.skyBox = new SkyBox(graphics.GraphicsDevice);

            if(NetworkManager != null)
                this.NetworkManager.Connect();

            Camera camera = new Camera((float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Width);
            this.camManager = new CameraManager(camera);

            this.ScoreBoard = new ScoreBoard(4);

            this.weaponManager = new WeaponManager(this);
            this.weaponManager.Init();
            this.projectileManager = new ProjectileManager(this, camera);
            this.playerManager = new PlayerManager(this);
                
            if (NetworkManager != null)
            {
                this.projectileManager.ShotFired += (sender, e) => this.NetworkManager.SendMessage(new ShotFiredMessage(e.Shot));
                this.playerManager.PlayerStateChanged += (sender, e) => this.NetworkManager.SendMessage(new UpdatePlayerStateMessage(e.Player));
                this.weaponManager.WeaponHasBeenCreated += (sender, e) => this.NetworkManager.SendMessage(new WeaponCreatedMessage(e.Weapon));
                //this.weaponManager.WeaponHasBeenLooted += (sender, e) => this.networkManager.SendMessage(new ShotFiredMessage(e.));
            }

            base.Initialize();
            Place = map.CreateWorld();

            if (this.IsHost)
            {
                localPlayerId = playerManager.GetCurrentAmountOfPlayers();
                HumanPlayer player = new HumanPlayer(localPlayerId, gameConfiguration.Health, gameConfiguration.Stamina,
                                                     gameConfiguration.Defense, gameConfiguration.MeleePower, gameConfiguration.RangePower,
                                                     gameConfiguration.Speed, 65, playerManager.nextSpawnPoint(), 5f / 60f, 50, 1f, 1f, 1f, true, this, camManager.ActiveCamera);
                player.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                player.addWeapon(weaponManager.GetWeapon(0));

                playerManager.AddPlayer(player);

                Vector3 chase = playerManager.GetPlayer(localPlayerId).Position;
                chase.Y = playerManager.GetPlayer(localPlayerId).CharacterController.Body.Height / 2;
                camera.SetTargetToChase(chase, playerManager.GetPlayer(localPlayerId).PlayerHeadOffset);
                if (this.networkManager == null)
                {
                    for (int i = 0; i < gameConfiguration.NumberOfCPUPlayers; i++)
                    {
                        long Id = playerManager.GetCurrentAmountOfPlayers();
                        AIPlayer play = new AIPlayer(Id, gameConfiguration.Health, gameConfiguration.Stamina,
                                                             gameConfiguration.Defense, gameConfiguration.MeleePower, gameConfiguration.RangePower,
                                                             gameConfiguration.Speed, 65, playerManager.nextSpawnPoint(), 5f / 60f, 50, 1f, 1f, 1f, false, this, gameConfiguration.AiDifficulty.ToString());
                        play.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                        play.addWeapon(weaponManager.GetWeapon(0));

                        playerManager.AddPlayer(play);
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            skyBox.Load(this.Content, "Textures\\SkyBoxes\\BlueSky\\SkyEffect", "Textures\\SkyBoxes\\BlueSky\\SkyBoxTex");
            font = Content.Load<SpriteFont>("Fonts\\gameFont");

            sexyTexture = Content.Load<Texture2D>("Textures\\lolbar");
            staminaTex = Content.Load<Texture2D>("Textures\\yellowGradient");
            healthTex = Content.Load<Texture2D>("Textures\\greenGradient");
            backBar = Content.Load<Texture2D>("Textures\\rock");
            infinity = Content.Load<Texture2D>("Textures\\infinity");
            
            LoadCharacterModels();
            LoadMap();

            gameManager.Init(this);

        }

        protected override void Update(GameTime gameTime)
        {
            weaponManager.Update();
            
            if(NetworkManager != null)
            {
                ProcessNetworkMessages();
            }
            space.Update();
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
                camManager.UpdateAllCameras(chase, playerManager.GetPlayer(localPlayerId).PlayerHeadOffset);
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            skyBox.Draw(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            map.Draw(graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            projectileManager.DrawAllBullets(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);          
            playerManager.DrawAllPlayers(gameTime, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            weaponManager.Draw(camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
            DrawUIComponents();
            

            base.Draw(gameTime);

        }

        private void LoadCharacterModels() 
        {
            Effect myEffect = Content.Load<Effect>("Effects\\skinFX");

            Model model;
            switch(gameConfiguration.Character)
            {
                case Helpers.States.Character.ClassicVincent:
                    model = Content.Load<Model>("Models\\Characters\\vincent");
                    break;
                case Helpers.States.Character.BlackVincent:
                    model = Content.Load<Model>("Models\\Characters\\vincent");
                    break;
                case Helpers.States.Character.BlueVincent:
                    model = Content.Load<Model>("Models\\Characters\\vincent");
                    break;
                case Helpers.States.Character.RedVincent:
                    model = Content.Load<Model>("Models\\Characters\\vincent");
                    break;
                default:
                    model = Content.Load<Model>("Models\\Characters\\vincent");
                    break;
            }

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    BasicEffect oldEffect = (BasicEffect)part.Effect;
                    Effect newEffect = myEffect.Clone();
                    newEffect.Parameters["Texture"].SetValue(oldEffect.Texture);

                    newEffect.Parameters["LightColor"].SetValue(new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
                    newEffect.Parameters["AmbientLightColor"].SetValue(new Vector4(1.25f, 1.25f, 1.25f, 1.0f));
                    newEffect.Parameters["Shininess"].SetValue(0.6f);
                    newEffect.Parameters["SpecularPower"].SetValue(0.4f);

                    newEffect.Parameters["View"].SetValue(camManager.ActiveCamera.ViewMatrix);
                    newEffect.Parameters["Projection"].SetValue(camManager.ActiveCamera.ProjectionMatrix);

                    part.Effect = newEffect;
                    oldEffect.Dispose();
                }
            }

        }

        private void LoadMap() 
        {
            switch(gameConfiguration.Map)
            {
                case Helpers.States.Maps.Map1:
                                this.map = new Terrain("Content\\Scenes\\Test.scn", this);
                                break;
                default:
                                this.map = new Terrain("Content\\Scenes\\Test.scn", this);
                                break;
            }
            this.map.LoadContent(Content);
            this.map.AddToSpace(space);
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
            spriteBatch.DrawString(font, "Local Player ID:" + localPlayerId, new Vector2(0,0), Color.White);

            spriteBatch.DrawString(font, "Local PlayerPosition:" + playerManager.GetPlayer(localPlayerId).Position.X + ":::::::" + playerManager.GetPlayer(localPlayerId).Position.Y + ":::::::" + playerManager.GetPlayer(localPlayerId).Position.Z, new Vector2(0, 10), Color.White);
            if (localPlayerId == 1 && playerManager.GetPlayer(0) != null)
            {

                spriteBatch.DrawString(font, "Remote PlayerPosition:" + playerManager.GetPlayer(0).Position.X + ":::::::" + playerManager.GetPlayer(0).Position.Y + ":::::::" + playerManager.GetPlayer(0).Position.Z, new Vector2(0, 20), Color.White);
            }
            else if (localPlayerId == 0 && playerManager.GetPlayer(1) != null)
                spriteBatch.DrawString(font, "Remote PlayerPosition:" + playerManager.GetPlayer(1).Position.X + ":::::::" + playerManager.GetPlayer(1).Position.Y + ":::::::" + playerManager.GetPlayer(1).Position.Z, new Vector2(0, 30), Color.White);

            spriteBatch.DrawString(font, "Weapons: " + weaponManager.ActiveWeapons.Count, new Vector2(0, 40), Color.White);
            
            if (playerManager.GetPlayer(localPlayerId).EquippedWeapons.Count != 0)
            {
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
            }

            spriteBatch.End();
        }

        private void ManageServerNetwork(object state) 
        {
            NetIncomingMessage incmsg;
            NetOutgoingMessage outmsg;

            while((incmsg = networkManager.ReadMessage()) != null)
            {
                switch(incmsg.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        incmsg.SenderConnection.Approve();

                        Player newRemotePlayer;

                        outmsg = networkManager.CreateMessage();
                        //new UpdatePlayerStateMessage(newRemotePlayer).Encode(outmsg);

                        break;
                    case NetIncomingMessageType.Data:
                        var gameMessageType = (GameMessageTypes)incmsg.ReadByte();
                        switch(gameMessageType)
                        {
                                //Client Requested For X
                            case GameMessageTypes.UpdatePlayerState:
                                break;
                            case GameMessageTypes.ShotFired:
                                break;
                            case GameMessageTypes.WeaponStateChange:
                                break;
                        }
                        
                        break;
                }
            }
        }

        private void ProcessNetworkMessages()
        {
            NetIncomingMessage im;

            while ((im = this.NetworkManager.ReadMessage()) != null)
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
                                    Console.WriteLine("Connected");
                                    var message = new UpdatePlayerStateMessage(im.SenderConnection.RemoteHailMessage);
                                    localPlayerId = message.Id;
                                    HumanPlayer player = new HumanPlayer(localPlayerId, 100, 100, 10, 100, 100, 30, 100, playerManager.nextSpawnPoint(), 5f / 60f, 50, 1f, 1f, 1f, true, this, camManager.ActiveCamera);
                                    player.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                                    player.addWeapon(weaponManager.GetWeapon(0));
                                    playerManager.AddPlayer(player);

                                    Vector3 chase = playerManager.GetPlayer(localPlayerId).Position;
                                    chase.Y = playerManager.GetPlayer(localPlayerId).CharacterController.Body.Height / 2;
                                    camManager.ActiveCamera.SetTargetToChase(chase, playerManager.GetPlayer(localPlayerId).PlayerHeadOffset);
                                    Console.WriteLine("Connected to {0}", im.SenderEndPoint);
                                }
                                else
                                {
                                    Console.WriteLine("{0} Connected", im.SenderEndPoint);
                                }

                                break;
                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine(this.IsHost ? "{0} Disconnected" : "Disconnected from {0}", im.SenderEndPoint);
                                break;

                            case NetConnectionStatus.RespondedAwaitingApproval:
                                NetOutgoingMessage hailMessage = this.NetworkManager.CreateMessage();
                                Player player1 = new Player(playerManager.GetCurrentAmountOfPlayers(), 100, 100, 100, 100, 100, 30, 100, new Vector3(0, 50, 0), 5f / 60f, 50, 1f, 1f, 1f, false);
                                player1.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                                playerManager.AddPlayer(player1);
                                

                                new UpdatePlayerStateMessage(player1).Encode(hailMessage);
                                im.SenderConnection.Approve(hailMessage);
                                

                                //Sending Weapons To Client
                                foreach (var temp in weaponManager.ActiveWeapons)
                                {
                                    NetOutgoingMessage message = this.networkManager.CreateMessage();
                                    new WeaponCreatedMessage(temp.Value.Item1).Encode(message);
                                    im.SenderConnection.SendMessage(message,NetDeliveryMethod.UnreliableSequenced,0);
                                   
                                }

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
                            case GameMessageTypes.WeaponStateChange:
                                this.HandleWeaponStateMessage(im);
                                break;
                        }

                        break;
                }

                this.NetworkManager.Recycle(im);
            }
        }

        private void HandleShotFiredMessage(NetIncomingMessage im)
        {
            var message = new ShotFiredMessage(im);

            if (message.FiredById != localPlayerId)
            {
                this.projectileManager.FireProjectile(playerManager.GetPlayer(message.FiredById), message.bulletDirection);
            }

        }

        private void HandleUpdatePlayerStateMessage(NetIncomingMessage im)
        {
            var message = new UpdatePlayerStateMessage(im);

            Player player;
            if (this.playerManager.GetPlayer(message.Id) != null)
            {
                player = this.playerManager.GetPlayer(message.Id);
            }
            else
            {
                player = new Player(message.Id, 100, 100, 100, 100, 100, 30, 100, message.Position, 5f / 60f, 30, 1f, 1f, 1f, false);
                player.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                playerManager.AddPlayer(player);
            }

            player.CharecterState = message.CharState;
            player.CharacterController.Body.LinearVelocity = message.Velocity;
            player.CharacterController.Body.Position = message.Position;
            player.Position = message.Position;
            player.Rotation = message.Rotation;
            player.addWeapon(weaponManager.GetWeapon((int)message.CurrentWeaponID));

        }

        private void HandleWeaponStateMessage(NetIncomingMessage im) 
        {
            var message = new WeaponCreatedMessage(im);

            if(message.WasCreated)
            {
                weaponManager.AddWeapon(message.WeaponPosition, message.WeaponID);
            }
            else if(!message.WasCreated)
            {
                weaponManager.pickupWeapon(message.WeaponID);
            }
        }

        #endregion

    }
}
