using plat_kill;
using plat_kill.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace plat_kill_server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //using (var game = new PlatKillGame())
            //{
            //    game.Run();
            //}

            using (var game = new PKGame(new ServerNetworkManager()))
            {
                game.Run();
            }
        }
    }
}
