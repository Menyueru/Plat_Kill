using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Managers
{
    public class ScoreBoard
    {
        public Dictionary<long, int> Score { get; set; }
        private long WinnerID;

        public ScoreBoard()
        {
            this.Score = new Dictionary<long, int>();
        }

        public void kill(long attackerID, long deadID)
        {
            if (Score.ContainsKey(attackerID))
            {
                Score[attackerID] += 1;
            }
            else
            {
                Score.Add(attackerID, 1);
            }
        }

        public String GetScoreBoard() 
        {
            string tempScore = "ScoreBoard";
            foreach(var temp in Score)
            {
                tempScore += System.Environment.NewLine + "Player#" + (temp.Key+1) + " have " + temp.Value + " points.";
            }
            
            return tempScore;
        }

        public long GetWinner() 
        {
            if (WinnerID != null)
            {
                return WinnerID;
            }
            else
            {
                long PlayerIdWinner = 0;
                foreach (var score in Score)
                {
                    if (score.Value > Score[PlayerIdWinner])
                    {
                        PlayerIdWinner = score.Key;
                    }
                }
                WinnerID = PlayerIdWinner;
                return PlayerIdWinner;
            }
        }
    }
}
