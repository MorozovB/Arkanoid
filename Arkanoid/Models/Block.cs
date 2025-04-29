using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Models
{
    internal class Block
    {
        // The hit points or durability of the block.
        internal int Capacity { get; set; }

        // The current state/color of the block.
        internal BlocksColor BlockColor { get; set; }

        // List of pixels forming this block (4 contiguous pixels horizontally).
        internal List<Pixel> BlockPixels { get; set; }

        // The top-left coordinate of the block.
        internal int X { get; private set; }
        internal int Y { get; private set; }

        // Indicates whether the block is visible.
        internal bool Visible { get; set; } = true;

        // Initializes a new block at the given top-left coordinate with the specified color.
        internal Block(int x, int y, BlocksColor color)
        {
            X = x;
            Y = y;
            BlockColor = color;
            SetCapacityAndColor(color);
            // Create a 4x1 block (4 pixels in a row).
            BlockPixels = new List<Pixel>
            {
                new Pixel(x, y, '■'),
                new Pixel(x + 1, y, '■'),
                new Pixel(x + 2, y, '■'),
                new Pixel(x + 3, y, '■')
            };
        }

        // Sets the capacity (durability) based on the block's color.
        private void SetCapacityAndColor(BlocksColor color)
        {
            switch (color)
            {
                case BlocksColor.Violet:
                    Capacity = 3;
                    break;
                case BlocksColor.Red:
                    Capacity = 2;
                    break;
                case BlocksColor.Yellow:
                    Capacity = 1;
                    break;
            }
        }

        // Changes the block's color based on its current capacity.
        internal void ChangeColor(int capacity)
        {
            switch (capacity)
            {
                case 2:
                    BlockColor = BlocksColor.Red;
                    break;
                case 1:
                    BlockColor = BlocksColor.Yellow;
                    break;
                    // Additional cases can be added if needed.
            }
        }

        // Draws the block on the console by drawing all four pixels with the appropriate color.
        internal void Draw()
        {
            switch (BlockColor)
            {
                case BlocksColor.Violet:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case BlocksColor.Red:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case BlocksColor.Yellow:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
            }

            foreach (Pixel pixel in BlockPixels)
            {
                pixel.Draw();
            }
            Console.ResetColor();
        }

        // Clears the block from the console by deleting all its pixels.
        internal void Clear()
        {
            foreach (Pixel pixel in BlockPixels)
            {
                pixel.DeletePixel();
            }
        }

        // Checks whether the given ball pixel is colliding with this block.
        // Collision is detected if ball.Y equals block.Y and ball.X lies in the interval [X, X+4).
        internal bool IsColliding(Pixel ball)
        {
            return ball.Y == Y && ball.X >= X && ball.X < X + 4;
        }
    }
}
