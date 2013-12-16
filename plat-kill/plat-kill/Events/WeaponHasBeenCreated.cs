using BEPUphysics.Entities.Prefabs;
using plat_kill.GameModels.Weapons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Events
{
    public class WeaponHasBeenCreated : EventArgs
    {
        public WeaponHasBeenCreated(Weapon weapon) 
        {
            this.Weapon = weapon;
        }

        public Weapon Weapon { get; private set; }
    }
}
