using Raylib_cs;
using SimpleRayCastingEngine.GameObjects;
using SimpleRayCastingEngine.Rendering;
using SimpleRayCastingEngine.Map; // Add this reference for WallType
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleRayCastingEngine
{
    public class Game
    {
        private Player player;
        private List<Enemy> enemies;

        // Update the map with wall types, using WallType enum values
        private int[,] map =
        {
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}, // 1 = Wall
            {1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1}, // 1 = Wall
            {1, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1}, // 2 = Water, more open space
            {1, 0, 3, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 1}, // 3 = Wood, path opened
            {1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1}, // 1 = Wall
            {1, 0, 1, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 1}, // 1 = Wall, path widened
            {1, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 1}, // 1 = Wall, more open space
            {1, 0, 1, 0, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 1}, // 1 = Wall
            {1, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 1, 1}, // 1 = Wall, more open area
            {1, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1}, // 1 = Wall, opening created
            {1, 0, 1, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 1}, // 1 = Wall
            {1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1}, // 1 = Wall
            {1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 0, 1, 1, 0, 1, 1}, // 1 = Wall
            {1, 0, 0, 0, 1, 0, 1, 0, 1, 1, 0, 0, 0, 0, 1, 1}, // 1 = Wall, opening added
            {1, 0, 1, 1, 0, 0, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0}, // 1 = Wall, more open area
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}  // 1 = Wall
        };

        public Game()
        {
            player = new Player(2, 2);
            enemies = new List<Enemy> { new Enemy(10, 10) };
        }

        public void Update()
        {
            // Handle player input and update the player state
            player.HandleInput(map);
            player.Update();

            // Update each enemy's state, passing the map for collision detection
            foreach (var enemy in enemies)
            {
                enemy.Update(player, map);  // Pass the map for each enemy to handle movement with collision
            }
        }

        public void Render()
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Render the scene with the updated map
            Renderer.RenderScene(player, map);  // Draw 3D world
            Renderer.RenderMinimap(player, enemies[0]);  // Draw 2D minimap

            Raylib.EndDrawing();
        }
    }
}
