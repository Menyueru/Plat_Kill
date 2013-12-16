using GameLauncher.SerializableObjects;
using GameLauncher.SerializableObjects.Preferences;
using GameLauncher.UIComponents;
using plat_kill;
using plat_kill.Helpers;
using plat_kill.Managers;
using plat_kill.Networking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher
{
    public partial class GameLauncher : MetroFramework.Forms.MetroForm
    {
        #region Fields
        private GameConfiguration gameConfiguration;

        private NotifyIcon notifyIcon;

        private int currentActiveUserControlIndex;
        private Stack<int> previousControlsIndexes;
        private List<UserControl> userControls;

        private const int mainScreenIndex = 0;
        private const int matchConfigurationScreenIndex = 1;
        private const int multiplayerScreenIndex = 2;
        private const int serverConfigScreenIndex = 3;
        private const int scoreBoardScreenIndex = 4;
        private const int generalConfigurationScreenIndex = 5;
        #endregion

        #region Propierties
        public GameConfiguration GameConfiguration
        {
            get { return gameConfiguration; }
            set { gameConfiguration = value; }
        }
        public int ServerConfigScreenIndex
        {
            get { return serverConfigScreenIndex; }
        }
        public int ScoreBoardScreenIndex
        {
            get { return scoreBoardScreenIndex; }
        }
        public int GeneralConfigurationScreenIndex
        {
            get { return generalConfigurationScreenIndex; }
        } 
        public int MatchConfigurationScreenIndex
        {
            get { return matchConfigurationScreenIndex; }
        }
        public int MainScreenIndex
        {
            get { return mainScreenIndex; }
        }
        public int MultiplayerScreenIndex
        {
            get { return multiplayerScreenIndex; }
        }
        #endregion

        public GameLauncher()
        {
            InitializeComponent();

            backButton.Hide();

            notifyIcon = new NotifyIcon();
            userControls = new List<UserControl>();
            previousControlsIndexes = new Stack<int>();

            PreferenceCollection pC = DeSerializer.DeserializePreferencesCollection("Resources\\Preferences\\preferences.xml");
            
            gameConfiguration = new GameConfiguration();
            gameConfiguration.ResolutionWidth = pC.Preferences[0].ResolutionWidth;
            gameConfiguration.ResolutionHeigth = pC.Preferences[0].ResolutionHeight;
            gameConfiguration.IsFullScreen = pC.Preferences[0].FullScreen;
            gameConfiguration.MasterVolume = pC.Preferences[0].MasterVolume;

            currentActiveUserControlIndex = 0;
            userControls.Add(new MainScreen(this)); //0
            userControls.Add(new MatchConfiguration(this)); //1
            userControls.Add(new MultiplayerScreen(this)); //2
            userControls.Add(new ServerConfigurations(this)); //3
            userControls.Add(new ScoreBoardScreen(this)); //4
            userControls.Add(new GeneralConfiguration(this)); //5

            mainPanel.Controls.Add(userControls[currentActiveUserControlIndex]);
        }

        #region Methods
        public void goToView(int index) 
        {
            mainPanel.Controls.Remove(userControls[currentActiveUserControlIndex]);

            previousControlsIndexes.Push(currentActiveUserControlIndex);
            currentActiveUserControlIndex = index;

            mainPanel.Controls.Add(userControls[currentActiveUserControlIndex]);

            if (currentActiveUserControlIndex > 0)
                backButton.Show();
            else
                backButton.Hide();
        }

        public void StartGame() 
        {
            MinimizeToSystemTray();

            Thread gameThread = new Thread(CallPKGame);
            gameThread.Start();
            gameThread.Join();

            MaximizeFromSystemTray();
        }

        private void CallPKGame()
        {
            using (PKGame game = new PKGame(gameConfiguration))
            {
                game.Run();
            }
        }

        private void MinimizeToSystemTray() 
        {
            notifyIcon.Visible = true;
            notifyIcon.BalloonTipText = "The Vincents Running!";
            notifyIcon.BalloonTipTitle = "Game On!";
            notifyIcon.BalloonTipIcon = ToolTipIcon.Warning;
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
            notifyIcon.ShowBalloonTip(20000);
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
        }

        private void MaximizeFromSystemTray() 
        {
            notifyIcon.Visible = false;
            this.WindowState = FormWindowState.Maximized;
            this.Show();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
           mainPanel.Controls.Remove(userControls[currentActiveUserControlIndex]);
           currentActiveUserControlIndex = previousControlsIndexes.Pop();
           mainPanel.Controls.Add(userControls[currentActiveUserControlIndex]);

            if(currentActiveUserControlIndex.Equals(0))
            {
                backButton.Hide();
            }
            
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            MaximizeFromSystemTray();
        }
        #endregion 
    }
}
