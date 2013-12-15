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
    public partial class ServerConfigurations : UserControl
    {
        private GameLauncher gameLauncher;
        public ServerConfigurations(GameLauncher parent)
        {
            InitializeComponent();
            this.gameLauncher = parent;
        }

        private void metroToolTip1_Popup(object sender, PopupEventArgs e)
        {

        }

        private void goButton_Click(object sender, EventArgs e)
        {
            if (ipTextBox.Text.Length > 0 && portTextBox.Text.Length > 0)
            {
                gameLauncher.GameConfiguration.NetworkManager = new ClientNetworkManager(this.ipTextBox.Text);
                gameLauncher.goToView(gameLauncher.MatchConfigurationScreenIndex);
            }
        }
    }
}
