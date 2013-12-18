using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Managers
{
    public class TimeMatch : IGameManager
    {
        public DateTime startTime {get; set;}
        public TimeSpan time {get; set;}

        private TimeSpan timeLeft 
        {
            get 
            {
                return startTime.Add(time).Subtract(DateTime.Now); 
            } 

            set 
            { 
                timeLeft = value; 
            } 
        }

        private DateTime finalTime { get; set; }
        private TimeSpan endtime { get; set; }

        private PKGame game;


        public TimeMatch(TimeSpan time)
        {
            this.time = time;
        }

        public void Init(PKGame game)
        {
            this.game = game;
            this.startTime = DateTime.Now;

            this.endtime = new TimeSpan(0,0,15);
        }

        public void Update()
        {
            
        }

        public void Pause()
        {
            
        }

        public string GetTimeLeft() 
        {
            TimeSpan _timeLeft = this.timeLeft;
            return _timeLeft.Hours + ":" + _timeLeft.Minutes + ":" + _timeLeft.Seconds;
        }

        public bool GameOver()
        {
            if (startTime.Add(time) < DateTime.Now)
            {
                finalTime = DateTime.Now;
                return true;
            }
            return false;
        }

        public bool ExitGameIn() 
        {
            if (finalTime.Add(endtime) < DateTime.Now)
            {
                return true;
            }
            return false;
        }
    }
}
