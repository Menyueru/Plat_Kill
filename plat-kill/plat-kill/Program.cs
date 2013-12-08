using plat_kill.Networking;
using System;
using plat_kill.Managers;

namespace plat_kill
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (PKGame game = new PKGame(new ServerNetworkManager(), new TimeMatch(new TimeSpan(0,0,10))))
            {
                game.Run();
            }
        }
    }
#endif
}

