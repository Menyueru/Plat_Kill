using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Managers
{
    public class ScoreBoard
    {
        private int[,] score;
        private int numPlayers;

        public int[,] Score
        {
            get { return score; }
        }

        public ScoreBoard(int numplayers)
        {
            this.numPlayers = numplayers;
            this.score= new int[this.numPlayers,this.numPlayers];
        }

        public void kill(long attackerID, long deadID)
        {
            this.score[attackerID, deadID]++;
        }
    }
}
