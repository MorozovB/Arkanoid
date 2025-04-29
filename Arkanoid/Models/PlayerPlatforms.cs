using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Models
{
    internal class PlayerPlatform
    {
        // The center of the platform.
        internal Pixel Center { get; private set; }
        // The list of pixels that form the platform.
        internal List<Pixel> Platform { get; private set; } = new List<Pixel>();

        // Default constructor.
        internal PlayerPlatform() { }

        // Initializes a new instance of the PlayerPlatform class.
        // initX - initial x coordinate of the platform's center.
        // initY - initial y coordinate of the platform's center.
        // platformLength - the length of the platform (default is 3, representing 3 pixels).
        internal PlayerPlatform(int initX, int initY, int platformLength = 3)
        {
            Center = new Pixel(initX, initY, '=');
            // For now, we are using a fixed platform pattern regardless of platformLength.
            UpdatePlatform();
            DrawPlatform();
        }

        // Moves the platform based on the arrow key input.
        // key - the key pressed by the user (LeftArrow or RightArrow).
        // consoleWidth - the width of the console, used to determine the moving boundaries.
        public void Move(ConsoleKey key, int consoleWidth)
        {
            // Clear current platform drawing.
            ClearPlatform();

            // Update the center position based on user input.
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    if (Center.X > 1)
                        Center = new Pixel(Center.X - 1, Center.Y, '=');
                    break;
                case ConsoleKey.RightArrow:
                    if (Center.X < consoleWidth - 2)
                        Center = new Pixel(Center.X + 1, Center.Y, '=');
                    break;
            }

            // Update and redraw the platform.
            UpdatePlatform();
            DrawPlatform();
        }

        // Clears the platform from the console.
        internal void ClearPlatform()
        {
            foreach (Pixel pixel in Platform)
            {
                pixel.DeletePixel();
            }
        }

        // Updates the platform's pixels based on the current center position.
        internal void UpdatePlatform()
        {
            Platform.Clear();
            // For a platform of length 3, add a pixel to the left, center, and right.
            Platform.Add(new Pixel(Center.X - 1, Center.Y, '='));
            Platform.Add(new Pixel(Center.X, Center.Y, '='));
            Platform.Add(new Pixel(Center.X + 1, Center.Y, '='));
        }

        // Draws the platform on the console.
        internal void DrawPlatform()
        {
            foreach (Pixel pixel in Platform)
            {
                pixel.Draw();
            }
        }
    }
}
