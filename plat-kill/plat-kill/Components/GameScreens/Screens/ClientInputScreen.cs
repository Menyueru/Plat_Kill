using Microsoft.Xna.Framework.Input;
using plat_kill.GameScreens.Screens;
using plat_kill.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Components.GameScreens.Screens
{
    class ClientInputScreen : MenuScreen
    {
        private MenuEntry IP1;
        private MenuEntry IP2;
        private MenuEntry IP3;
        private MenuEntry IP4;
        private MenuEntry continueMenuEntry;

        private int firstOct;
        private int secondOct;
        private int thirdOct;
        private int fourthOct;
        
        public ClientInputScreen()
            : base("IP: 0 . 0 . 0 . 0 ")
        {
            this.IP1 = new MenuEntry(String.Empty);
            this.IP2 = new MenuEntry(String.Empty);
            this.IP3 = new MenuEntry(String.Empty);
            this.IP4 = new MenuEntry(String.Empty);
            this.continueMenuEntry = new MenuEntry(String.Empty);

            this.firstOct = 127;
            this.secondOct = 0;
            this.thirdOct = 0;
            this.fourthOct = 1;

            this.IP1.Selected += IP1MenuEntrySelected;
            this.IP2.Selected += IP2MenuEntrySelected;
            this.IP3.Selected += IP3MenuEntrySelected;
            this.IP4.Selected += IP4MenuEntrySelected;
            this.continueMenuEntry.Selected += continueEntrySelected;

            this.MenuEntries.Add(IP1);
            this.MenuEntries.Add(IP2);
            this.MenuEntries.Add(IP3);
            this.MenuEntries.Add(IP4);
            this.MenuEntries.Add(continueMenuEntry);

            this.IP1.Text = "10";
            this.IP2.Text = "0";
            this.IP3.Text = "0";
            this.IP4.Text = "4";
            this.continueMenuEntry.Text = "Connect!";

        }

        void IP1MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Add))
            {
                if((this.firstOct + 1)  <= 255)
                    this.firstOct += 1;
            }
            else if (keyState.IsKeyDown(Keys.Subtract))
            {
                if ((this.firstOct - 1) >= 0)
                    this.firstOct -= 1;
            }

            
            SetMenuEntryText();
        }

        void IP2MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Add))
            {
                if ((this.secondOct + 1) <= 255)
                    this.secondOct += 1;
            }
            else if (keyState.IsKeyDown(Keys.Subtract))
            {
                if ((this.secondOct - 1) >= 0)
                    this.secondOct -= 1;
            };

            SetMenuEntryText();
        }

        void IP3MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Add))
            {
                if ((this.thirdOct + 1) <= 255)
                    this.thirdOct += 1;
            }
            else if (keyState.IsKeyDown(Keys.Subtract))
            {
                if ((this.thirdOct - 1) >= 0)
                    this.thirdOct -= 1;
            }

            SetMenuEntryText();
        }

        void IP4MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Add))
            {
                if ((this.fourthOct + 1) <= 255)
                    this.fourthOct += 1;
            }
            else if (keyState.IsKeyDown(Keys.Subtract))
            {
                if ((this.fourthOct - 1) >= 0)
                    this.fourthOct -= 1;
            }

            SetMenuEntryText();
        }

        void continueEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen(new ClientNetworkManager(firstOct.ToString()+ "." + secondOct.ToString() + "." +  
                                                                           thirdOct.ToString() + "." + fourthOct.ToString())));
        }

        void SetMenuEntryText()
        {
            this.menuTitle = "IP: " + firstOct + "." + secondOct + "." + thirdOct + "." + fourthOct;
            this.IP1.Text = firstOct.ToString();
            this.IP2.Text = secondOct.ToString();
            this.IP3.Text = thirdOct.ToString();
            this.IP4.Text = fourthOct.ToString();
        }
    }
}
