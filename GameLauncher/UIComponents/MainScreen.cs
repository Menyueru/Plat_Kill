using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameLauncher.UIComponents
{
    public partial class MainScreen : UserControl
    {
        private GameLauncher gameLauncher;

        public MainScreen(GameLauncher parent)
        {
            InitializeComponent();
            this.gameLauncher = parent;
        }

        private void singlePlayerTile_Click(object sender, EventArgs e)
        {
            gameLauncher.GameConfiguration.NetworkManager = null;
            ((MatchConfiguration)gameLauncher.UserControls[gameLauncher.MatchConfigurationScreenIndex]).CheckGameMode();
            gameLauncher.goToView(gameLauncher.MatchConfigurationScreenIndex);
            gameLauncher.GameConfiguration.NetworkManager = null;
        }

        private void multiPlayerTile_Click(object sender, EventArgs e)
        {
            gameLauncher.goToView(gameLauncher.MultiplayerScreenIndex);
        }

        private void configTile_Click(object sender, EventArgs e)
        {
            gameLauncher.goToView(gameLauncher.GeneralConfigurationScreenIndex);
        }
    }
}
