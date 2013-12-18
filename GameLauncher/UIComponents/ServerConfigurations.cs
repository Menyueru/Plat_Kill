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
using System.Net;

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
            IPAddress address;
            if (IPAddress.TryParse(ipTextBox.Text, out address))
            {
                try
                {
                    var temp = new ClientNetworkManager(address.ToString());
                    temp.Connect();
                    //temp.Disconnect();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Can't find any active game on the given addres. Try again Later.", "No game room found.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                gameLauncher.GameConfiguration.NetworkManager = new ClientNetworkManager(address.ToString());
                ((MatchConfiguration)gameLauncher.UserControls[gameLauncher.MatchConfigurationScreenIndex]).CheckGameMode();
                gameLauncher.goToView(gameLauncher.MatchConfigurationScreenIndex);
            }
            else 
            {
                MessageBox.Show("Invalid string. Try with something that have a similar format to 0.0.0.0 .", "Try Again!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
