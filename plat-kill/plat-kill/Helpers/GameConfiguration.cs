using System;
using plat_kill.Networking;
using plat_kill.Managers;
using plat_kill.Helpers.States;

namespace plat_kill.Helpers
{
    public class GameConfiguration
    {
        #region Fields
        private IGameManager gameManager;

        private AIDifficulty aiDifficulty;
        private int numberOfCPUPlayers;
        private INetworkManager networkManager;

        private Maps map;
        private Character character;
        private long health;
        private long stamina;
        private long defense;
        private long meleePower;
        private long rangePower;
        private long speed;

        private int masterVolume;
        private int resolutionWidth;
        private int resolutionHeigth;
        private bool isFullScreen;

        #endregion

        #region Propierties
        public int MasterVolume
        {
            get { return masterVolume; }
            set { masterVolume = value; }
        }
        public long Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public long Health
        {
            get { return health; }
            set { health = value; }
        }
        public long Stamina
        {
            get { return stamina; }
            set { stamina = value; }
        }
        public long Defense
        {
            get { return defense; }
            set { defense = value; }
        }
        public long MeleePower
        {
            get { return meleePower; }
            set { meleePower = value; }
        }
        public long RangePower
        {
            get { return rangePower; }
            set { rangePower = value; }
        }
        public IGameManager GameManager
        {
            get { return gameManager; }
            set { gameManager = value; }
        }
        public AIDifficulty AiDifficulty
        {
            get { return aiDifficulty; }
            set { aiDifficulty = value; }
        }
        public int NumberOfCPUPlayers
        {
            get { return numberOfCPUPlayers; }
            set { numberOfCPUPlayers = value; }
        }
        public INetworkManager NetworkManager
        {
            get { return networkManager; }
            set { networkManager = value; }
        }
        public Maps Map
        {
            get { return map; }
            set { map = value; }
        }
        public Character Character
        {
            get { return character; }
            set { character = value; }
        }
        public int ResolutionWidth
        {
            get { return resolutionWidth; }
            set { resolutionWidth = value; }
        }
        public int ResolutionHeigth
        {
            get { return resolutionHeigth; }
            set { resolutionHeigth = value; }
        }
        public bool IsFullScreen
        {
            get { return isFullScreen; }
            set { isFullScreen = value; }
        }
        #endregion

        public GameConfiguration() 
        {
            this.health = 0;
            this.defense = 0;
            this.speed = 0;
            this.stamina = 0;
            this.meleePower = 0;
            this.rangePower = 0;
        }
    }
}
