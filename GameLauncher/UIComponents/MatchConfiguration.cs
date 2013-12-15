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
            gameLauncher.GameConfiguration.Speed = charCollection.Characters[charCollectionCurrentIndex].Speed;
            gameLauncher.GameConfiguration.Health = charCollection.Characters[charCollectionCurrentIndex].Health;
            gameLauncher.GameConfiguration.Defense = charCollection.Characters[charCollectionCurrentIndex].Defense;
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
            gameLauncher.GameConfiguration.GameManager = new TimeMatch(new TimeSpan(0, 10, 0));
            gameLauncher.GameConfiguration.Health = 100;
            gameLauncher.GameConfiguration.Stamina = 100;
            gameLauncher.GameConfiguration.MeleePower = 100;
            gameLauncher.GameConfiguration.RangePower = 100;
            gameLauncher.GameConfiguration.Speed = 100;

            gameLauncher.GameConfiguration.NumberOfCPUPlayers = Convert.ToInt32(numEnemiesLabel.Text);
            
            switch(difficultyLabel.Text)
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

            gameLauncher.GameConfiguration.Map = plat_kill.Helpers.States.Maps.Map1;

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
        #endregion


    }
}
