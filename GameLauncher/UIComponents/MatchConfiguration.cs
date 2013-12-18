using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using plat_kill.Managers;
using GameLauncher.SerializableObjects;
using GameLauncher.SerializableObjects.Maps;
using GameLauncher.SerializableObjects.Characters;
using GameLauncher.SerializableObjects.Preferences;
using plat_kill.Networking;


namespace GameLauncher.UIComponents
{
    public partial class MatchConfiguration : UserControl
    {
        #region Fields
        private GameLauncher gameLauncher;

        private MapCollection mapCollection;
        private CharacterCollection charCollection;

        private int charCollectionCurrentIndex;
        #endregion

        public MatchConfiguration(GameLauncher parent)
        {
            InitializeComponent();
            this.gameLauncher = parent;

            this.difficultyTrackBar.Value = 0;
            this.enemiesTrackBar.Value = 0;

            this.mapCollection = DeSerializer.DeserializeMapCollection("Resources\\Maps\\map.xml");
            this.charCollection = DeSerializer.DeserializeCharacterCollection("Resources\\Characters\\characters.xml");

            this.charCollectionCurrentIndex = 0;

            foreach (MapSeriazable ms in mapCollection.Maps)
            {
                this.mapComboBox.Items.Add(ms.Name);
            }

            this.mapComboBox.SelectedIndex = 0;

            SetCurrentCharacter();
        }

        #region Methods
        private void SetCurrentCharacter() 
        {
            this.charNameLbl.Text = charCollection.Characters[charCollectionCurrentIndex].Name;
            this.meleePowerLabel.Text = charCollection.Characters[charCollectionCurrentIndex].MeleePower.ToString();
            this.rangePowrLabel.Text = charCollection.Characters[charCollectionCurrentIndex].RangePower.ToString();
            this.staminaLabel.Text = charCollection.Characters[charCollectionCurrentIndex].Stamina.ToString();
            this.speedLabel.Text = charCollection.Characters[charCollectionCurrentIndex].Speed.ToString();
            this.defenseLabel.Text = charCollection.Characters[charCollectionCurrentIndex].Defense.ToString();
            this.healthLabel.Text = charCollection.Characters[charCollectionCurrentIndex].Health.ToString();

            switch (charCollection.Characters[charCollectionCurrentIndex].Name)
            {
                case "RedVincent":
                    gameLauncher.GameConfiguration.Character = plat_kill.Helpers.States.Character.RedVincent;
                    break;
                case "BlueVincent":
                    gameLauncher.GameConfiguration.Character = plat_kill.Helpers.States.Character.BlueVincent;
                    break;
                case "BlackVincent":
                    gameLauncher.GameConfiguration.Character = plat_kill.Helpers.States.Character.BlackVincent;
                    break;
                case "ClassicVincent":
                    gameLauncher.GameConfiguration.Character = plat_kill.Helpers.States.Character.ClassicVincent;
                    break;
            }

            gameLauncher.GameConfiguration.MeleePower = charCollection.Characters[charCollectionCurrentIndex].MeleePower;
            gameLauncher.GameConfiguration.RangePower = charCollection.Characters[charCollectionCurrentIndex].RangePower;
            gameLauncher.GameConfiguration.Health = charCollection.Characters[charCollectionCurrentIndex].Health;
            gameLauncher.GameConfiguration.Defense = charCollection.Characters[charCollectionCurrentIndex].Defense;
            gameLauncher.GameConfiguration.Stamina = charCollection.Characters[charCollectionCurrentIndex].Stamina;
            gameLauncher.GameConfiguration.Speed = charCollection.Characters[charCollectionCurrentIndex].Speed;
        }

        private void NextCharacter()
        {
            if ((charCollectionCurrentIndex + 1) < charCollection.Characters.Length)
            {
                charCollectionCurrentIndex += 1;
            }
            else 
            {
                charCollectionCurrentIndex = 0;
            }
            SetCurrentCharacter();
        }

        private void PreviousCharacter()
        {
            if ((charCollectionCurrentIndex - 1) >= 0)
            {
                charCollectionCurrentIndex -= 1;
            }
            else
            {
                charCollectionCurrentIndex = charCollection.Characters.Length - 1;
            }
            SetCurrentCharacter();
        }

        private void goTile_Click(object sender, EventArgs e)
        {
            if (gameLauncher.GameConfiguration.NetworkManager == null || gameLauncher.GameConfiguration.NetworkManager.GetType().Equals(typeof(ServerNetworkManager)))
            {
                if (timeBox.Text == "" || timeBox.Text == " ")
                {
                    MessageBox.Show("Please specify the duration of the match.", "Missing duration.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                if (Convert.ToInt32(timeBox.Text) > 30 && Convert.ToInt32(timeBox.Text) < 0)
                {
                    MessageBox.Show("The duration must be a value between 0 and 30 minutes.", "Bad duration.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                gameLauncher.GameConfiguration.GameManager = new TimeMatch(new TimeSpan(0, Convert.ToInt32(timeBox.Text), 0));
                gameLauncher.GameConfiguration.NumberOfCPUPlayers = Convert.ToInt32(numEnemiesLabel.Text);

                switch (difficultyLabel.Text)
                {
                    case "Easy":
                        gameLauncher.GameConfiguration.AiDifficulty = plat_kill.Helpers.States.AIDifficulty.Easy;
                        break;
                    case "Medium":
                        gameLauncher.GameConfiguration.AiDifficulty = plat_kill.Helpers.States.AIDifficulty.Medium;
                        break;
                    case "Hard":
                        gameLauncher.GameConfiguration.AiDifficulty = plat_kill.Helpers.States.AIDifficulty.Hard;
                        break;
                    default:
                        gameLauncher.GameConfiguration.AiDifficulty = plat_kill.Helpers.States.AIDifficulty.Easy;
                        break;
                }
                if (this.mapComboBox.SelectedIndex == 0)
                {
                    gameLauncher.GameConfiguration.Map = plat_kill.Helpers.States.Maps.Map1;
                }
                else if (this.mapComboBox.SelectedIndex == 1)
                {
                    gameLauncher.GameConfiguration.Map = plat_kill.Helpers.States.Maps.Map2;
                }
                else
                {
                    gameLauncher.GameConfiguration.Map = plat_kill.Helpers.States.Maps.Map3;
                }
                
            }
            else
            {
                gameLauncher.GameConfiguration.GameManager = new TimeMatch(new TimeSpan(0, Convert.ToInt32(20), 0));
            }
            gameLauncher.StartGame();
        }

        private void nextCharButton_Click(object sender, EventArgs e)
        {
            NextCharacter();
        }

        private void previousCharButton_Click(object sender, EventArgs e)
        {
            PreviousCharacter();
        }
        
        private void difficultyTrackBar_ValueChanged(object sender, EventArgs e)
        {
            float value = this.difficultyTrackBar.Value;
            if(value >= 0 && value < 33)
            {
                this.difficultyLabel.Text = "Easy";
            }
            else if (value >= 33 && value < 66)
            {
                this.difficultyLabel.Text = "Medium";
            }
            else if(value >= 66)
            {
                this.difficultyLabel.Text = "Hard";
            }


        }

        private void enemiesTrackBar_ValueChanged(object sender, EventArgs e)
        {
            float value = this.enemiesTrackBar.Value;
            if (value == 0)
            {
                this.numEnemiesLabel.Text = "0";
            }
            else if (value > 0 && value < 33)
            {
                this.numEnemiesLabel.Text = "1";
            }
            else if (value >= 33 && value < 66)
            {
                this.numEnemiesLabel.Text = "2";
            }
            else if (value >= 66)
            {
                this.numEnemiesLabel.Text = "3";
            }

        }

        public void CheckGameMode() 
        {
            if (gameLauncher.GameConfiguration.NetworkManager != null)
            {
                if (gameLauncher.GameConfiguration.NetworkManager.GetType().Equals(typeof(ClientNetworkManager)))
                {
                    this.goTile.Height = 200;
                    this.goTile.Location = new Point(7, 29);
                    this.metroPanel2.Hide();
                    this.metroPanel4.Hide();
                }
                else
                {
                    this.goTile.Height = 34;
                    this.goTile.Location = new Point(7, 197);
                    this.metroPanel2.Show(); 
                    this.metroPanel4.Show();
                }
            }
            else
            {
                this.goTile.Height = 34;
                this.goTile.Location = new Point(7, 197);
                this.metroPanel2.Show();
                this.metroPanel4.Show();
            }
        }

        private void timeBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            const char Delete = (char)8;
            e.Handled = !Char.IsDigit(e.KeyChar) && e.KeyChar != Delete;
        }
        #endregion

      


    }
}
