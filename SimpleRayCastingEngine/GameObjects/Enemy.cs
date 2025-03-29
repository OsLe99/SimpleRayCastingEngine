using Raylib_cs;
using System;
using System.Numerics;

namespace SimpleRayCastingEngine.GameObjects
{
    public class Enemy
    {
        public Vector2 Position { get; private set; }
        private float speed = 0.05f;
        private float shootCooldown = 60;
        private float shootTimer = 0;

        public Enemy(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        public void Update(Player player, int[,] map)
        {
            MoveToward(player, map); // Pass the map for collision detection
            ShootAt(player);
        }

        private void MoveToward(Player player, int[,] map)
        {
            Vector2 direction = Vector2.Normalize(player.Position - Position);
            Vector2 newPosition = Position + direction * speed;

            // Check for collisions at the new position before updating
            if (map[(int)newPosition.X, (int)newPosition.Y] == 0) // Assuming 0 means empty space
            {
                Position = newPosition; // Update only if no collision
            }
        }

        private void ShootAt(Player player)
        {
            if (shootTimer > 0)
            {
                shootTimer--;
                return;
            }

            shootTimer = shootCooldown;
            Console.WriteLine("Enemy fires at the player!");
        }

        public void Draw()
        {
            Raylib.DrawCircle((int)Position.X * 10, (int)Position.Y * 10, 5, Color.Red); // Draw enemy on the map
        }
    }
}
