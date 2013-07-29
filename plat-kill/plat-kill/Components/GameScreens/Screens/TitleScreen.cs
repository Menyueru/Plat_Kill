using System;
using System.Collections.Generic;
using plat_kill.GameScreens.ScreenComponents;


namespace plat_kill.GameScreens.Screens
{
    class TitleScreen : MenuScreen
    {

        public TitleScreen() 
            : base("ISC - Prototipo del Proyecto Final")
        {
            MenuEntry continueMenuEntry = new MenuEntry("Press any key to Continue...");

            continueMenuEntry.Selected += continueGameMenuEntrySelected;

            MenuEntries.Add(continueMenuEntry);
        }

        void continueGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new MainMenuScreen(), e.PlayerIndex);

        }

    }
}
