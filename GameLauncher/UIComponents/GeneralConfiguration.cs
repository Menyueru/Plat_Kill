using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameLauncher.SerializableObjects.Preferences;
using GameLauncher.SerializableObjects;

namespace GameLauncher.UIComponents
{
    public partial class GeneralConfiguration : UserControl
    {
        #region Fields
        private GameLauncher gameLauncher;
        private PreferenceCollection pC;
        private List<Tuple<int, int>> resolutions;
        #endregion
        
        public GeneralConfiguration(GameLauncher parent)
        {
            InitializeComponent();

            this.gameLauncher = parent;

            pC = DeSerializer.DeserializePreferencesCollection("Resources\\Preferences\\preferences.xml");
            this.resolutionComboBox.Items.Add(new Tuple<int, int>(Convert.ToInt32(pC.Preferences[0].ResolutionWidth),
                                                                  Convert.ToInt32(pC.Preferences[0].ResolutionHeight)));

            resolutions = new List<Tuple<int, int>>();
            resolutions.Add(new Tuple<int, int>(1366, 768));
            resolutions.Add(new Tuple<int, int>(1280, 768));
            resolutions.Add(new Tuple<int, int>(1024, 768));

            foreach(var tuple in resolutions)
            {
                if(!resolutionComboBox.Items.Contains(tuple))
                    this.resolutionComboBox.Items.Add(tuple);
            }

            this.resolutionComboBox.SelectedIndex = 0;
            this.fullscreenToggle.Checked = pC.Preferences[0].FullScreen;
            this.tabControl.SelectedIndex = 0;
        }

        #region Methods
        private void resolutionComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            Tuple<int, int> temp = (Tuple<int, int>)this.resolutionComboBox.SelectedItem;
            pC.Preferences[0].ResolutionWidth = temp.Item1;
            pC.Preferences[0].ResolutionHeight = temp.Item2;

            gameLauncher.GameConfiguration.ResolutionWidth = temp.Item1;
            gameLauncher.GameConfiguration.ResolutionHeigth = temp.Item2;

            DeSerializer.SerializeObject(pC, "Resources\\Preferences\\preferences.xml");
        }
        private void fullscreenToggle_CheckedChanged(object sender, EventArgs e)
        {
            pC.Preferences[0].FullScreen = fullscreenToggle.Checked;
            gameLauncher.GameConfiguration.IsFullScreen = fullscreenToggle.Checked;
            DeSerializer.SerializeObject(pC, "Resources\\Preferences\\preferences.xml");
        }

        private void metroTrackBar1_ValueChanged(object sender, EventArgs e)
        {
            pC.Preferences[0].MasterVolume = metroTrackBar1.Value;
            gameLauncher.GameConfiguration.MasterVolume = metroTrackBar1.Value;
            DeSerializer.SerializeObject(pC, "Resources\\Preferences\\preferences.xml");
        }
        #endregion 


    }
}
