using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Models
{
    internal class Pixel
    {
        // Default constructor.
        public Pixel() { }

        // Constructs a new Pixel with specified x, y coordinates and character.
        // x - The x-coordinate of the pixel.
        // y - The y-coordinate of the pixel.
        // pixelChar - The character that represents the pixel.
        public Pixel(int x, int y, char pixelChar)
        {
            X = x;
            Y = y;
            PixelChar = pixelChar;
        }

        // The x-coordinate of the pixel.
        internal int X { get; set; }
        // The y-coordinate of the pixel.
        internal int Y { get; set; }
        // The character to be displayed for the pixel.
        internal char PixelChar { get; set; }

        // Draws the pixel on the console if its coordinates are within window bounds.
        internal void Draw()
        {
            if (IsWithinBounds())
            {
                Console.SetCursorPosition(X, Y);
                Console.Write(PixelChar);
            }
        }

        // Clears the pixel from the console by replacing it with a space if within bounds.
        internal void DeletePixel()
        {
            if (IsWithinBounds())
            {
                Console.SetCursorPosition(X, Y);
                Console.Write(' ');
            }
        }

        // Checks whether the pixel is within the current console window boundaries.
        private bool IsWithinBounds()
        {
            return X >= 0 && X < Console.WindowWidth &&
                   Y >= 0 && Y < Console.WindowHeight;
        }
    }
}
