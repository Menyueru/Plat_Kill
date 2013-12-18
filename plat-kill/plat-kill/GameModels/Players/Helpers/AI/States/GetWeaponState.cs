using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.GameModels.Players.Helpers.AI.States
{
    public class GetWeaponState :IState
    {
        private Vector2 target;
        private long targetID;
        public void Start(AIPlayer bot)
        {
            var wManager = bot.Game.WeaponManager;
            if (wManager.ActiveWeapons.Count > 0)
            {
                float min = 9999999f;
                long bestkey = 0;
                foreach (var weapon in wManager.ActiveWeapons.Keys)
                {
                    float result = Vector3.Distance(bot.Position, wManager.ActiveWeapons[weapon].Item2.Position);
                    min = Math.Min(min, result);
                    if (min == result) bestkey = weapon;
                }

                bot.Crumb2 = PathFinder.FindPath(bot.Game.Place, new Point3D(bot.Position), new Point3D(wManager.ActiveWeapons[bestkey].Item2.Position));
                bot.Crumb2 = bot.Crumb2.next;
                if (bot.Crumb2.next != null)
                {
                    bot.gotWeapon = false;
                    bot.MovingTowards = bot.Crumb2.position.toVector2();
                    target = new Vector2(wManager.ActiveWeapons[bestkey].Item2.Position.X, wManager.ActiveWeapons[bestkey].Item2.Position.Z);
                    targetID = bestkey;
                }
            }
            else
            {
                bot.MovingTowards = new Vector2(bot.Position.X, bot.Position.Z);
            }
        }

        public void Update(AIPlayer bot)
        {
            if (!bot.Game.WeaponManager.ActiveWeapons.ContainsKey(targetID)) 
                this.Start(bot);
            if (bot.Crumb2 != null && bot.Crumb2.next!= null)
            {
                bot.Crumb2 = bot.Crumb2.next;
                if (bot.Crumb2.position == new Point3D(bot.Position))
                {
                   
                    if (bot.Crumb2.next == null)
                    {
                        bot.MovingTowards = new Vector2(bot.Position.X, bot.Position.Z);
                        return;
                    }
                    bot.MovingTowards = bot.Crumb2.position.toVector2();
                }
            }
            else
            {
                if (bot.gotWeapon)
                {
                    Start(bot);
                }
                else
                {
                    bot.MovingTowards = target;
                }
            }


        }

        public void End(AIPlayer bot)
        {
            if (!bot.gotWeapon) bot.gotWeapon = true;
            bot.Crumb2 = null;
        }
    }
}
