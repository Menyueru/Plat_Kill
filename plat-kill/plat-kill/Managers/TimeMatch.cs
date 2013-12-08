using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Managers
{
    class TimeMatch : IGameManager
    {
        private DateTime startTime;
        private TimeSpan time;
        private PKGame game;

        public TimeMatch(TimeSpan time)
        {
            this.time = time;
        }

        public void Init(PKGame game)
        {
            this.game = game;
            this.startTime = DateTime.Now;
        }

        public void Update()
        {
            
        }

        public void Pause()
        {
            
        }

        public bool GameOver()
        {
            if (startTime.Add(time) < DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
