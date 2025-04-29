using Arkanoid.Models;
using Arkanoid.Utils;
using Arkanoid.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkanoid.Core
{
    internal class Game
    {
        // Player's platform.
        internal PlayerPlatform GamePlatform { get; set; }
        // The game ball.
        internal Ball GameBall { get; set; }
        // List of blocks.
        internal List<Block> Blocks { get; set; } = new List<Block>();
        // Remaining lives.
        internal int Lives { get; set; } = 3;
        // Player's score.
        internal int Points { get; set; } = 0;

        // Player's name.
        private string playerName = "Player";
        // Flag for pausing the game.
        private bool isPaused = false;
        // Flag to track previous state of the 'P' key.
        private bool pKeyPreviouslyDown = false;

        // Constructor: initialize console settings, game objects and build blocks.
        internal Game(string title)
        {
            // Set console size to 65x20.
            GameSettings.WindowsSettings(65, 20, title);

            // Reserve bottom 2 rows for UI.
            // Place game objects within rows 0..(20 - 2 = 18).
            // Platform will be at row 16 and ball at row 14.
            GamePlatform = new PlayerPlatform(Console.WindowWidth / 2, Console.WindowHeight - 4); // 20 - 4 = 16
            GameBall = new Ball(Console.WindowWidth / 2, Console.WindowHeight - 6, 1);              // 20 - 6 = 14

            CreateBlocks();
            BuildBlocks();
        }

        // Asks the player for their name.
        internal void AskPlayerName()
        {
            Console.Clear();
            Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 - 1);
            Console.Write("Enter your name: ");
            Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2);
            playerName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(playerName))
                playerName = "Player";
        }

        // Displays the main menu.
        internal void ShowMenu()
        {
            Console.Clear();
            string[] menuItems = { "▶ Play", "Scores" };
            int selected = 0;
            while (true)
            {
                Console.Clear();
                Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 2 - 3);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("ARKANOID");
                Console.ResetColor();

                for (int i = 0; i < menuItems.Length; i++)
                {
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 5, Console.WindowHeight / 2 + i);
                    if (i == selected)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"> {menuItems[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"  {menuItems[i]}");
                    }
                }
                Thread.Sleep(100);

                if (Keyboard.IsKeyDown(ConsoleKey.UpArrow))
                    selected = (selected - 1 + menuItems.Length) % menuItems.Length;
                else if (Keyboard.IsKeyDown(ConsoleKey.DownArrow))
                    selected = (selected + 1) % menuItems.Length;
                else if (Keyboard.IsKeyDown(ConsoleKey.Enter))
                {
                    switch (selected)
                    {
                        case 0:
                            Console.Clear();
                            return;
                        case 1:
                            ShowScores();
                            break;
                    }
                }
                // Wait until keys are released.
                while (Keyboard.IsKeyDown(ConsoleKey.UpArrow) ||
                       Keyboard.IsKeyDown(ConsoleKey.DownArrow) ||
                       Keyboard.IsKeyDown(ConsoleKey.Enter))
                {
                    Thread.Sleep(50);
                }
            }
        }

        // Starts the game by launching the ball loop in a separate thread and processing input.
        internal void Start()
        {
            Thread ballThread = new Thread(BallLoop)
            {
                IsBackground = true
            };
            ballThread.Start();

            while (true)
            {
                HandleKeyboardInput();
                Thread.Sleep(20);
            }
        }

        // Handles user input for moving the platform and toggling pause.
        private void HandleKeyboardInput()
        {
            if (Keyboard.IsKeyDown(ConsoleKey.LeftArrow))
                GamePlatform.Move(ConsoleKey.LeftArrow, Console.WindowWidth);
            else if (Keyboard.IsKeyDown(ConsoleKey.RightArrow))
                GamePlatform.Move(ConsoleKey.RightArrow, Console.WindowWidth);

            bool pDown = Keyboard.IsKeyDown(ConsoleKey.P);
            if (pDown && !pKeyPreviouslyDown)
            {
                isPaused = !isPaused;
                lock (ConsoleHelper.ConsoleLock)
                {
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 3, Console.WindowHeight / 2);
                    if (isPaused)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("PAUSE");
                        Console.ResetColor();
                    }
                    else
                        Console.Write("     ");
                }
            }
            pKeyPreviouslyDown = pDown;
        }

        // Main loop handling ball movement, collision detection, and full scene redraw.
        private void BallLoop()
        {
            while (true)
            {
                if (!isPaused)
                {
                    // Handle boundary collisions.
                    if (GameBall.PlayBall.X <= 0 || GameBall.PlayBall.X >= Console.WindowWidth - 1)
                        GameBall.BounceX();
                    if (GameBall.PlayBall.Y <= 0)
                        GameBall.BounceY();

                    // Update ball position.
                    GameBall.Move();
                    BounceFromPlatform();
                    BounceFromBlocks();
                    CheckMissedBall();

                    // Check if all blocks have been destroyed.
                    if (CheckVictory())
                    {
                        // Add bonus: 100 points per remaining life.
                        Points += 100 * Lives;
                        lock (ConsoleHelper.ConsoleLock)
                        {
                            Console.Clear();
                            Console.SetCursorPosition((Console.WindowWidth / 2) - 20, Console.WindowHeight / 2);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Congratulations! You cleared all blocks! Final Score: {Points}");
                            SaveScoreToFile();
                            Console.ResetColor();
                        }
                        Thread.Sleep(5000);
                        Environment.Exit(0);
                    }

                    // Full redraw of the scene.
                    lock (ConsoleHelper.ConsoleLock)
                    {
                        Console.Clear();
                        BuildBlocks();              // Redraw blocks.
                        GamePlatform.DrawPlatform(); // Redraw platform.
                        GameBall.Draw();             // Draw ball.
                        DrawUI();                    // Draw UI.
                    }
                }
                Thread.Sleep(150);
            }
        }

        // Creates blocks arranged in a grid in the upper part of the game area.
        internal void CreateBlocks()
        {
            Blocks.Clear();
            int cellWidth = 5;  // 4 for the block plus 1 pixel gap.
            int cellHeight = 1; // 1 for the block plus 1 pixel gap.
            int numColumns = Console.WindowWidth / cellWidth;
            int numRows = 5;    // For example, create 3 rows of blocks.

            Random random = new Random();
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    int x = col * cellWidth;
                    int y = row * cellHeight;
                    Array values = Enum.GetValues(typeof(BlocksColor));
                    BlocksColor randomColor = (BlocksColor)values.GetValue(random.Next(values.Length));
                    Blocks.Add(new Block(x, y, randomColor));
                }
            }
        }

        // Draws all visible blocks.
        internal void BuildBlocks()
        {
            foreach (var block in Blocks)
            {
                if (block.Visible)
                    block.Draw();
            }
        }

        // Updates the state (drawing or clearing) of blocks based on their capacity.
        internal void UpdateBlocks()
        {
            foreach (var block in Blocks)
            {
                if (block.Capacity <= 0)
                {
                    block.Visible = false;
                    block.Clear();
                }
                else
                {
                    block.Draw();
                }
            }
        }

        // Handles collision between the ball and the player's platform.
        // Checks if the ball is exactly one row above the platform.
        internal void BounceFromPlatform()
        {
            if (GameBall.PlayBall.Y == GamePlatform.Center.Y - 1)
            {
                if (GameBall.PlayBall.X >= GamePlatform.Center.X - 1 &&
                    GameBall.PlayBall.X <= GamePlatform.Center.X + 1)
                {
                    GameBall.BounceY();
                }
            }
        }

        // Detects collisions between the ball and blocks using the updated 4x1 block size.
        internal void BounceFromBlocks()
        {
            foreach (Block block in Blocks)
            {
                if (!block.Visible)
                    continue;

                if (block.IsColliding(GameBall.PlayBall))
                {
                    // Calculate center of the block:
                    // For a 4x1 block, center X = X + 2 and center Y = Y.
                    double blockCenterX = block.X + 2.0;
                    double blockCenterY = block.Y;
                    double diffX = GameBall.PlayBall.X - blockCenterX;
                    double diffY = GameBall.PlayBall.Y - blockCenterY;

                    // Bounce depending on the penetration (choose axis with larger difference).
                    if (Math.Abs(diffX) > Math.Abs(diffY))
                        GameBall.BounceX();
                    else if (Math.Abs(diffY) > Math.Abs(diffX))
                        GameBall.BounceY();
                    else
                    {
                        GameBall.BounceX();
                        GameBall.BounceY();
                    }
                    HitBlock(block);
                    break;  // Process one collision per frame.
                }
            }
        }

        // Processes a hit on a block.
        private void HitBlock(Block block)
        {
            block.Capacity--;
            block.ChangeColor(block.Capacity);
            Points++;
            UpdateBlocks();
        }

        // Checks if the ball has been missed by the platform.
        // If the ball goes at least one position below the platform, restart the game state.
        private void CheckMissedBall()
        {
            if (GameBall.PlayBall.Y >= GamePlatform.Center.Y + 1)
            {
                Lives--;
                if (Lives <= 0)
                {
                    lock (ConsoleHelper.ConsoleLock)
                    {
                        Console.Clear();
                        Console.SetCursorPosition((Console.WindowWidth / 2) - 5, Console.WindowHeight / 2 - 1);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("GAME OVER");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.SetCursorPosition((Console.WindowWidth / 2) - 7, Console.WindowHeight / 2 + 1);
                        Console.WriteLine($"YOUR SCORE: {Points}");
                        SaveScoreToFile();
                        Console.ResetColor();
                    }
                    Thread.Sleep(3000);
                    Environment.Exit(0);
                }
                ResetGameState();
            }
        }

        // Checks whether all blocks have been destroyed.
        private bool CheckVictory()
        {
            foreach (var block in Blocks)
            {
                if (block.Visible)
                    return false;
            }
            return true;
        }

        // Resets the game state after a missed ball.
        private void ResetGameState()
        {
            // Reinitialize positions: platform at row 16 and ball at row 14.
            GamePlatform = new PlayerPlatform(Console.WindowWidth / 2, Console.WindowHeight - 4);
            GameBall = new Ball(Console.WindowWidth / 2, Console.WindowHeight - 6, 1);
        }

        // Draws the game UI (score, lives, player name) in the reserved bottom area.
        private void DrawUI()
        {
            // Draw a horizontal separator at row 18.
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(new string('*', Console.WindowWidth));

            // Display player info at row 19.
            Console.SetCursorPosition(2, Console.WindowHeight - 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Player: {playerName}  Lives: {Lives}  Score: {Points}");
            Console.ResetColor();
        }

        // Saves the score to a file.
        private void SaveScoreToFile()
        {
            string scoreEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {playerName} — {Points} points";
            File.AppendAllText("scores.txt", scoreEntry + Environment.NewLine);
        }

        // Displays the high scores.
        private void ShowScores()
        {
            Console.Clear();
            string path = "scores.txt";
            Console.SetCursorPosition(Console.WindowWidth / 2 - 7, 2);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("High Scores");
            Console.ResetColor();

            if (File.Exists(path))
            {
                string[] scores = File.ReadAllLines(path);
                int startY = 4;
                for (int i = 0; i < scores.Length && i < Console.WindowHeight - 2; i++)
                {
                    Console.SetCursorPosition(4, startY + i);
                    Console.Write(scores[i]);
                }
            }
            else
            {
                Console.SetCursorPosition(4, 5);
                Console.Write("No scores recorded yet.");
            }
            Console.SetCursorPosition(4, Console.WindowHeight - 1);
            Console.Write("Press Escape to return to the menu.");
            while (!Keyboard.IsKeyDown(ConsoleKey.Escape))
                Thread.Sleep(50);
            Console.Clear();
        }
    }
}
