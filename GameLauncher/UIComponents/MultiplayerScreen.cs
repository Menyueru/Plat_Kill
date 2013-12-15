using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using plat_kill.Networking;

namespace GameLauncher.UIComponents
{
    public partial class MultiplayerScreen : UserControl
    {
        private GameLauncher gameLauncher;

        public MultiplayerScreen(GameLauncher parent)
        {
            InitializeComponent();
            this.gameLauncher = parent;
        }

        private void metroTile2_Click(object sender, EventArgs e)
        {
            gameLauncher.goToView(gameLauncher.MatchConfigurationScreenIndex);
            gameLauncher.GameConfiguration.NetworkManager = new ServerNetworkManager();
        }

        private void metroTile1_Click(object sender, EventArgs e)
        {
            gameLauncher.goToView(gameLauncher.ServerConfigScreenIndex);
        }
    }
}
