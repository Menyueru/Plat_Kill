using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.GameModels.Players.Helpers.AI.States
{
    class DeadState : IState
    {
        public void Start(AIPlayer bot)
        {
            bot.Target = null;
            bot.Crumb2 = null;
        }

        public void Update(AIPlayer bot)
        {
            if (!bot.IsDead)
            {
                bot.StateManager.ChangeState(new GetWeaponState());
            }
        }

        public void End(AIPlayer bot)
        {
            bot.MovingTowards = new Vector2(bot.Position.X, bot.Position.Z);
        }
    }
}
