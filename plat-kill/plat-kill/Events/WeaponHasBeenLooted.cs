using plat_kill.GameModels.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Events
{
    public class WeaponHasBeenLooted : EventArgs
    {
        public WeaponHasBeenLooted(Weapon weapon) 
        {
            this.Weapon = weapon;
        }

        public Weapon Weapon { get; set; }

    }
}
