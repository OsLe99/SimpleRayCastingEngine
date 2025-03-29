using Raylib_cs;
using System;
using System.Numerics;

namespace SimpleRayCastingEngine.GameObjects
{
    public class Player
    {
        public Vector2 Position { get; private set; }
        public float Angle { get; private set; }
        private float moveSpeed = 0.1f;
        private float turnSpeed = 0.05f;

        public int AmmoCount { get; private set; } = 2;
        private bool isReloading = false;
        private int reloadFrames = 0;

        public Player(float x, float y)
        {
            Position = new Vector2(x, y);
            Angle = 0f;
        }

        // Use this method to rotate the player based on Q and E input
        public void HandleInput(int[,] map)
        {
            Vector2 newPosition = Position;

            // Movement logic (WASD)
            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                newPosition.X += MathF.Cos(Angle) * moveSpeed;
                newPosition.Y += MathF.Sin(Angle) * moveSpeed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.S))
            {
                newPosition.X -= MathF.Cos(Angle) * moveSpeed;
                newPosition.Y -= MathF.Sin(Angle) * moveSpeed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.D))
            {
                newPosition.X -= MathF.Sin(Angle) * moveSpeed;
                newPosition.Y += MathF.Cos(Angle) * moveSpeed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.A))
            {
                newPosition.X += MathF.Sin(Angle) * moveSpeed;
                newPosition.Y -= MathF.Cos(Angle) * moveSpeed;
            }

            // Turning logic (Q/E)
            if (Raylib.IsKeyDown(KeyboardKey.Q))
            {
                Angle -= turnSpeed;
                if (Angle < 0) Angle += MathF.PI * 2;  // Normalize angle to [0, 2π)
            }
            if (Raylib.IsKeyDown(KeyboardKey.E))
            {
                Angle += turnSpeed;
                if (Angle >= MathF.PI * 2) Angle -= MathF.PI * 2;  // Normalize angle to [0, 2π)
            }

            // Update position if not colliding
            if (map[(int)newPosition.X, (int)newPosition.Y] == 0)
            {
                Position = newPosition;
            }

            // Shooting input (Spacebar)
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                Shoot();
            }
        }

        private void Shoot()
        {
            if (AmmoCount > 0)
            {
                AmmoCount--;
                Console.WriteLine("Bang! Ammo left: " + AmmoCount);
            }
            else if (!isReloading)
            {
                isReloading = true;
                reloadFrames = 60;
            }
        }

        public void Update()
        {
            if (isReloading && reloadFrames > 0)
            {
                reloadFrames--;
                if (reloadFrames == 0)
                {
                    isReloading = false;
                    AmmoCount = 2;
                }
            }
        }
    }
}
