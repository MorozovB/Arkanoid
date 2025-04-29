using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Models
{
    internal class Ball
    {
        public Ball() { }

        // Initializes a new instance of the Ball class with specified starting position and power.
        // x - the starting x coordinate of the ball.
        // y - the starting y coordinate of the ball.
        // power - the power of the ball (reserved for future use).
        internal Ball(int x, int y, int power)
        {
            PlayBall = new Pixel(x, y, 'O');
            Power = power;
        }

        // The pixel representation of the ball.
        internal Pixel PlayBall { get; set; }

        // The power of the ball.
        internal int Power { get; set; }

        // Horizontal direction of the ball's movement.
        internal int DirectionX { get; set; } = 1;

        // Vertical direction of the ball's movement.
        internal int DirectionY { get; set; } = 1;

        // Moves the ball based on its current direction.
        internal void Move()
        {
            // Clear the previous position of the ball.
            Clear();

            // Update the ball's position.
            PlayBall.X += DirectionX;
            PlayBall.Y += DirectionY;

            // Draw the ball at its new position.
            Draw();
        }

        // Draws the ball on the console.
        internal void Draw()
        {
            PlayBall.Draw();
        }

        // Clears the previous drawing of the ball from the console.
        internal void Clear()
        {
            PlayBall.DeletePixel();
        }

        // Inverts the horizontal direction, simulating a bounce off a vertical surface.
        internal void BounceX()
        {
            DirectionX *= -1;
        }

        // Inverts the vertical direction, simulating a bounce off a horizontal surface.
        internal void BounceY()
        {
            DirectionY *= -1;
        }
    }
}
