using plat_kill;
using plat_kill.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace plat_kill_client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(3000);
            using (var game = new PKGame(new ClientNetworkManager()))
            {
                game.Run();
            }
        }
    }
}
