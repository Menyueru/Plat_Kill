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
using BEPUphysics.Entities.Prefabs;
using plat_kill.Helpers.Serializable.Weapons;
using System.Net.Sockets;
using System.Net;

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
        public SoundManager soundManager { get; private set; }
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

            graphics.PreferredBackBufferWidth = 800;//gameConfiguration.ResolutionWidth;
            graphics.PreferredBackBufferHeight = 600;// gameConfiguration.ResolutionHeigth;
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

            Camera camera = new Camera((float)graphics.GraphicsDevice.Viewport.Width / (float)graphics.GraphicsDevice.Viewport.Width);
            this.camManager = new CameraManager(camera);

            this.ScoreBoard = new ScoreBoard(4);

            this.weaponManager = new WeaponManager(this);
            this.weaponManager.Init();
            this.projectileManager = new ProjectileManager(this, camera);
            this.playerManager = new PlayerManager(this);
            this.soundManager = new SoundManager(this, this.gameConfiguration.MasterVolume);

            if (NetworkManager != null)
            {
                this.projectileManager.ShotFired += (sender, e) => this.NetworkManager.SendMessage(new ShotFiredMessage(e.Shot));
                this.playerManager.PlayerStateChanged += (sender, e) => this.NetworkManager.SendMessage(new UpdatePlayerStateMessage(e.Player));
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
                    AIPlayer play = new AIPlayer(localPlayerId + 1, gameConfiguration.Health, gameConfiguration.Stamina,
                                                         gameConfiguration.Defense, gameConfiguration.MeleePower, gameConfiguration.RangePower,
                                                         gameConfiguration.Speed, 65, playerManager.nextSpawnPoint(), 5f / 60f, 50, 1f, 1f, 1f, false, this);
                    play.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                    play.addWeapon(weaponManager.GetWeapon(0));

                    playerManager.AddPlayer(play);
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

            if (NetworkManager != null)
                this.NetworkManager.Connect();

            gameManager.Init(this);

            soundManager.StartBackgroundMusic();
        }

        protected override void Update(GameTime gameTime)
        {
            if (!gameManager.GameOver())
            {
                weaponManager.Update();
                soundManager.Update();

                if (NetworkManager != null)
                {
                    ProcessNetworkMessages();
                }

                space.Update();
                gameManager.Update();

                playerManager.UpdateAllPlayers(gameTime);
                projectileManager.UpdateAllBullets();

                if (playerManager.GetPlayer(localPlayerId) != null)
                {
                    Vector3 chase = playerManager.GetPlayer(localPlayerId).Position;
                    camManager.UpdateAllCameras(chase, playerManager.GetPlayer(localPlayerId).PlayerHeadOffset);
                }
            }
            else 
            {
                if(((TimeMatch)gameManager).ExitGameIn())
                {
                    this.Exit();
                }

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

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            this.soundManager.StopBackgroundMusic();

            if(networkManager != null)
            {
                networkManager.Disconnect();
            }
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


            if(networkManager != null && networkManager.GetType().Equals(typeof(ServerNetworkManager)))
                spriteBatch.DrawString(font, "Server IP: " + System.Environment.NewLine + Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString(), new Vector2(10, graphics.GraphicsDevice.Viewport.Height - 50), Color.Black);

            spriteBatch.DrawString(font, "Time Left" + System.Environment.NewLine +((TimeMatch)gameManager).GetTimeLeft(), new Vector2(10, 50), Color.Red);
            spriteBatch.DrawString(font, ScoreBoard.GetScoreBoard(), new Vector2(10, 150), Color.Red);
            
            if(gameManager.GameOver())
            {
                if (ScoreBoard.GetWinner().Equals(localPlayerId))
                {
                    spriteBatch.DrawString(font, "You Win!!!", new Vector2(graphics.GraphicsDevice.Viewport.Width/2, graphics.GraphicsDevice.Viewport.Height/2), Color.Red);
                    this.soundManager.PlaySoundFX(Helpers.States.SoundEffects.YouWin);
                }
                else 
                {
                    spriteBatch.DrawString(font, "You Loose!!!", new Vector2(graphics.GraphicsDevice.Viewport.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2), Color.Red);
                    this.soundManager.PlaySoundFX(Helpers.States.SoundEffects.YouLoose);
                }
            }
            
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

        private void ProcessNetworkMessages()
        {
            NetIncomingMessage im;

            while ((im = this.NetworkManager.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)im.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                if (!this.IsHost)
                                {
                                    var message = new NewPlayerJoined(im.SenderConnection.RemoteHailMessage);
                                    localPlayerId = message.Players[0].PlayerID;
                                    playerManager.LocalPlayer = localPlayerId;
                                    
                                    HumanPlayer player = new HumanPlayer(localPlayerId, 100, 100, 10, 100, 100, 30, 100, new Vector3(message.Players[0].PosX, message.Players[0].PosY, message.Players[0].PosZ), 5f / 60f, 50, 1f, 1f, 1f, true, this, camManager.ActiveCamera);
                                    player.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                                    player.addWeapon(weaponManager.GetWeapon(0));
                                    playerManager.AddPlayer(player);

                                    for (int i = 1; i < message.Players.Count; i++)
                                    {
                                        Player RemotePlayer = new Player(message.Players[i].PlayerID, 100, 100, 100, 100, 100, 30, 100, new Vector3(message.Players[i].PosX, message.Players[i].PosY, message.Players[i].PosZ), 5f / 60f, 50, 1f, 1f, 1f, false);
                                        RemotePlayer.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);    
                                        playerManager.AddPlayer(RemotePlayer);
                                    }

                                    Vector3 chase = playerManager.GetPlayer(localPlayerId).Position;
                                    chase.Y = playerManager.GetPlayer(localPlayerId).CharacterController.Body.Height / 2;
                                    camManager.ActiveCamera.SetTargetToChase(chase, playerManager.GetPlayer(localPlayerId).PlayerHeadOffset);
                                }

                                break;
                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine(this.IsHost ? "{0} Disconnected" : "Disconnected from {0}", im.SenderEndPoint);
                                break;

                            case NetConnectionStatus.RespondedAwaitingApproval:
                                Player player1 = new Player(playerManager.GetCurrentAmountOfPlayers(), 100, 100, 100, 100, 100, 30, 100, playerManager.nextSpawnPoint(), 5f / 60f, 50, 1f, 1f, 1f, false);
                                player1.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                                playerManager.AddPlayer(player1);

                                List<Player> tempPlayers = new List<Player>();
                                tempPlayers.Add(player1);
                                foreach(Player p in playerManager.Players)
                                {
                                    tempPlayers.Add(p);
                                }

                                NetOutgoingMessage hailMessage = this.NetworkManager.CreateMessage();
                                new NewPlayerJoined(tempPlayers).Encode(hailMessage);
                                im.SenderConnection.Approve(hailMessage);

                                networkManager.SendMessage(new NewPlayerJoined(tempPlayers));

                                ((ServerNetworkManager)networkManager).SendMessageToSingleClient(new WeaponUpdateMessage(weaponManager.ActiveWeapons), im.SenderConnection);
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
                            case GameMessageTypes.NewPlayerJoined:
                                this.HandleNewPlayerJoined(im);
                                break;
                            case GameMessageTypes.TimeUpdate:
                                this.HandleTimeUpdate(im);
                                break;
                            case GameMessageTypes.ScoreUpdate:
                                this.HandleScoreUpdate(im);
                                break;
                        }

                        break;
                }
                if (networkManager.GetType().Equals(typeof(ServerNetworkManager))) 
                {
                    if (this.weaponManager.newWeaponsHaveBeenAdded) 
                    {
                        this.weaponManager.newWeaponsHaveBeenAdded = false;
                        var mess = new WeaponUpdateMessage(weaponManager.ActiveWeapons);
                        networkManager.SendMessage(mess);
                    }

                    networkManager.SendMessage(new TimeUpdate(((TimeMatch)gameManager).startTime, ((TimeMatch)gameManager).time));
                    networkManager.SendMessage(new ScoreUpdate(this.ScoreBoard.Score));
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
            if(localPlayerId != message.Id)
            {
                if (networkManager.GetType().Equals(typeof(ServerNetworkManager)))
                {
                    networkManager.SendMessage(message);
                }

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
                player.addWeapon(weaponManager.GetWeapon(message.CurrentWeaponID));
                player.Health = message.Health;
            }

        }

        private void HandleWeaponStateMessage(NetIncomingMessage im) 
        {
            var message = new WeaponUpdateMessage(im);
            if(message.activeWeapons != null)
            {
                Dictionary<long, Tuple<Weapon, Box>> tempDic = new Dictionary<long,Tuple<Weapon,Box>>();
                
                foreach(var element in message.activeWeapons)
                {
                    Box box = new Box(new Vector3(element.Value.Item2.PosX, element.Value.Item2.PosY, element.Value.Item2.PosZ), element.Value.Item2.Width, element.Value.Item2.Heigth, element.Value.Item2.Width);
                    box.Tag = element.Value.Item2.Tag;
                    tempDic.Add(element.Key, new Tuple<Weapon, Box>(weaponManager.GetWeapon(element.Value.Item1, box), box));
                }
                weaponManager.ActiveWeapons = tempDic;
            }
        }

        private void HandleNewPlayerJoined(NetIncomingMessage im) 
        {
            var message = new NewPlayerJoined(im);

            foreach(var p in message.Players)
            {
                if(playerManager.GetPlayer(p.PlayerID) == null)
                {
                    Player temp = new Player(p.PlayerID, 100, 100, 100, 100, 100, 30, 100, new Vector3(p.PosX, p.PosY, p.PosZ), 5f / 60f, 50, 1f, 1f, 1f, false);
                    temp.Load(this.Content, "Models\\Characters\\vincent", space, graphics.GraphicsDevice, camManager.ActiveCamera.ViewMatrix, camManager.ActiveCamera.ProjectionMatrix);
                    playerManager.AddPlayer(temp);
                }
            }
        }

        private void HandleTimeUpdate(NetIncomingMessage im)
        {
            var message = new TimeUpdate(im);

            if(this.gameManager.GetType().Equals(typeof(TimeMatch)))
            {
                ((TimeMatch)gameManager).startTime = message.Time.Item1;
                ((TimeMatch)gameManager).time = message.Time.Item2;
            }
        }

        private void HandleScoreUpdate(NetIncomingMessage im) 
        {
            var message = new ScoreUpdate(im);

            this.ScoreBoard.Score = message.Score;
        }

        #endregion

    }
}
