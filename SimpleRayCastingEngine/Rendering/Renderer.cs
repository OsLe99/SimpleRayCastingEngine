using Raylib_cs;
using SimpleRayCastingEngine.GameObjects;
using SimpleRayCastingEngine.Map; // Add this for WallType and WallColors
using System;
using System.Numerics;

namespace SimpleRayCastingEngine.Rendering
{
    public static class Renderer
    {
        public static void RenderScene(Player player, int[,] map)
        {
            int screenWidth = Raylib.GetScreenWidth();
            int screenHeight = Raylib.GetScreenHeight();
            int numRays = screenWidth;  // One ray per pixel column
            float fov = MathF.PI / 3;   // 60-degree field of view
            float halfFov = fov / 2;
            float maxDepth = 100;        // Maximum ray distance

            for (int x = 0; x < numRays; x++)
            {
                // Use the player's Angle for raycasting
                float rayAngle = (player.Angle - halfFov) + (x / (float)numRays) * fov;

                float rayX = MathF.Cos(rayAngle);
                float rayY = MathF.Sin(rayAngle);

                float distance = 0;
                bool hitWall = false;

                Vector2 rayPos = player.Position;

                // Raycasting loop: move forward until a wall is hit
                while (!hitWall && distance < maxDepth)
                {
                    distance += 0.1f; // Step size

                    int mapX = (int)(rayPos.X + rayX * distance);
                    int mapY = (int)(rayPos.Y + rayY * distance);

                    if (mapX < 0 || mapX >= map.GetLength(0) || mapY < 0 || mapY >= map.GetLength(1))
                    {
                        hitWall = true;
                    }
                    else
                    {
                        // Get the wall type based on the map value (e.g., 1, 2, 3...)
                        WallType wallType = (WallType)map[mapX, mapY];

                        if (wallType != WallType.Empty) // Hit a wall (non-empty)
                        {
                            hitWall = true;

                            // Get the wall color based on its type
                            Color wallColor = WallColors.GetColor(wallType);

                            // Calculate wall height based on distance (simple perspective projection)
                            int wallHeight = (int)(screenHeight / distance);

                            // Calculate shading (darker walls for further distances)
                            byte shade = (byte)(255 / (1 + distance));

                            // Apply shading to the wall color
                            wallColor = new Color(
                                (byte)(wallColor.R * shade / 255),
                                (byte)(wallColor.G * shade / 255),
                                (byte)(wallColor.B * shade / 255),
                                wallColor.A  // Alpha remains the same
                            );

                            // Draw vertical line for this ray with the appropriate color
                            Raylib.DrawLine(x, (screenHeight - wallHeight) / 2, x, (screenHeight + wallHeight) / 2, wallColor);
                        }
                    }
                }
            }

            // Debug: Draw the player's facing direction
            Vector2 direction = new Vector2(MathF.Cos(player.Angle), MathF.Sin(player.Angle)) * 50; // Length of the line (adjust as needed)
            Raylib.DrawLineV(player.Position, player.Position + direction, Color.Red);  // Red line showing direction
        }

        public static void RenderMinimap(Player player, Enemy enemy)
        {
            const int minimapSize = 250;
            const int tileSize = 10;

            Raylib.DrawRectangle(0, 0, minimapSize, minimapSize, Color.Black);

            // Draw player
            Raylib.DrawCircle((int)(player.Position.X * tileSize), (int)(player.Position.Y * tileSize), 3, Color.Red);

            // Draw enemy
            Raylib.DrawCircle((int)(enemy.Position.X * tileSize), (int)(enemy.Position.Y * tileSize), 3, Color.Blue);
        }
    }
}
