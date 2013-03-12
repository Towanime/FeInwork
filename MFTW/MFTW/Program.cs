using System;

namespace FeInwork
{
#if WINDOWS || XBOX
    static class Program
    {
        public static GameClass GAME;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (GAME = new GameClass())
            {
                GAME.Run();
            }
        }
    }
#endif
}

