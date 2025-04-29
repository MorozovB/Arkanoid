using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Core
{
    internal static class GameSettings
    {
        internal static void WindowsSettings(int screenWidth, int screenHeight, string name)
        {
            Console.Title = name;
            try
            {
                Console.SetWindowSize(screenWidth, screenHeight);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to set window or buffer size: " + ex.Message);
            }
            Console.CursorVisible = false;
        }
    }
}
