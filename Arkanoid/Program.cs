using Arkanoid.Core;

namespace Arkanoid
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game("Arkanoid");
            game.AskPlayerName();
            game.ShowMenu();
            game.Start();
        }
    }
}
