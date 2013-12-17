using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.GameModels.Players.Helpers.AI.States
{
    public interface IState
    {   
        void Start(AIPlayer bot);
        void Update(AIPlayer bot);
        void End(AIPlayer bot);
    }
}
