using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace plat_kill.Managers
{
    public interface IGameManager
    {
        void Init(PKGame game);
        void Update();
        void Pause();
        bool GameOver();
    }
}
