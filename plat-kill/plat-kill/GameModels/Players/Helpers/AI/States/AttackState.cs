using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.GameModels.Players.Helpers.AI.States
{
    public class AttackState : IState
    {
        public void Start(AIPlayer bot)
        {
            long MAXID= 0;
            float MAX=0;
            foreach(var weapon in bot.EquippedWeapons)
            {
                if (weapon.LoadedAmmo > 0 && weapon.WeaponType!= Weapons.WeaponType.Melee)
                {
                    MAX = Math.Max(MAX, weapon.WeaponDamage);
                    if (MAX == weapon.WeaponDamage)
                        MAXID = weapon.WeaponID;
                }
            }
            bot.ActiveWeaponIndex =(int) MAXID;
        }

        public void Update(AIPlayer bot)
        {
            if (bot.EquippedWeapons[bot.ActiveWeaponIndex] != null)
            {   if(bot.ActiveWeaponIndex >= 0 || bot.ActiveWeaponIndex < bot.EquippedWeapons.Count)
                {
                    if (bot.EquippedWeapons[bot.ActiveWeaponIndex].LoadedAmmo > 0)
                        this.Start(bot);
                }

            }
            if (!bot.Target.IsDead)
            {
                Vector3 dir = bot.Target.Position - bot.Position;
                dir.Normalize();
                Vector3 bulletDir = dir;
                bulletDir.Normalize();
                bot.EquippedWeapons[bot.ActiveWeaponIndex].Shoot(bot.Game.ProjectileManager, bot, bulletDir);
                float radianAngle = (float)Math.Acos(Vector3.Dot(dir, bot.World.Backward));
                if (Math.Abs(radianAngle) > 0.1)
                {
                    bot.Rotation -= new Vector3(0, radianAngle, 0);
                }
            }
            else
            {
                bot.Target = null;
                bot.StateManager.ChangeState(new GetWeaponState());
            }
            
        }


        public void End(AIPlayer bot)
        {
            if (bot.Target!=null && (bot.Target.IsDead || bot.Target.Health<=0))
            {
                bot.Target = null;
                
            }
            if (!bot.IsDead && bot.Health > 0)
            {
                bot.LastHit = bot.Id;
            }
        }
    }
}
