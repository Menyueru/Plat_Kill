using plat_kill.Networking;
using System;

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
            using (PKGame game = new PKGame(new ServerNetworkManager()))
            {
                game.Run();
            }
        }
    }
#endif
}

