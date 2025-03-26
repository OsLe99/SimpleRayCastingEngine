using System;
using Raylib_cs;
using System.Numerics;
using System.Collections.Generic;

namespace RaycastingExample
{
    class Program
    {
        const int screenWidth = 1280;  // Wider screen
        const int screenHeight = 720;  // Standard height for widescreen (16:9 aspect ratio)
        const int mapWidth = 16;
        const int mapHeight = 16;

        static int[,] map = new int[mapWidth, mapHeight]
        {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
            {1,0,0,1,1,0,1,0,0,0,0,0,0,0,1,1},
            {1,0,1,0,0,0,1,0,1,1,0,0,0,1,1,1},
            {1,0,1,0,1,1,1,0,0,0,0,1,0,0,1,1},
            {1,0,1,0,1,1,1,1,1,0,0,1,0,1,1,1},
            {1,0,0,0,1,0,0,1,1,0,1,1,0,0,0,1},
            {1,0,1,0,1,0,1,1,1,0,0,0,0,0,1,1},
            {1,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1},
            {1,0,1,1,0,1,1,1,0,0,0,1,0,0,1,1},
            {1,0,1,0,0,0,1,0,0,1,1,1,0,1,1,1},
            {1,0,1,1,0,0,1,0,1,0,1,1,0,0,0,1},
            {1,0,1,1,0,1,0,0,0,1,1,1,1,0,0,1},
            {1,0,0,0,1,0,1,0,1,1,0,0,0,0,1,1},
            {1,0,1,1,1,0,0,1,0,0,1,1,0,1,0,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
        };

        static float playerX = 2.0f;
        static float playerY = 2.0f;
        static float playerAngle = 0.0f;
        static float moveSpeed = 0.1f;
        static float turnSpeed = 0.05f;

        static int ammoCount = 2;
        static bool isReloading = false;
        static int reloadFrames = 0;
        static int fireFrames = 0;

        static List<(float x, float y, int frames)> particles = new List<(float x, float y, int frames)>();

        static void Main(string[] args)
        {
            Raylib.InitWindow(screenWidth, screenHeight, "Raycasting Example");
            Raylib.SetTargetFPS(60);

            while (!Raylib.WindowShouldClose())
            {
                HandleInput();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DarkGray);

                // Draw the raycasted world
                CastRays();

                // Draw the minimap in the top-left corner (250px by 250px)
                RenderMinimap();

                // Draw Ammo Counter underneath the minimap
                DrawAmmoCounter();

                // Draw the shotgun in the lower half of the screen
                DrawShotgun();

                // Draw particles
                DrawParticles();

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        static void HandleInput()
        {
            float newX = playerX;
            float newY = playerY;

            // Movement logic
            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                newX += MathF.Cos(playerAngle) * moveSpeed;
                newY += MathF.Sin(playerAngle) * moveSpeed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.S))
            {
                newX -= MathF.Cos(playerAngle) * moveSpeed;
                newY -= MathF.Sin(playerAngle) * moveSpeed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.D))
            {
                newX -= MathF.Sin(playerAngle) * moveSpeed;
                newY += MathF.Cos(playerAngle) * moveSpeed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.A))
            {
                newX += MathF.Sin(playerAngle) * moveSpeed;
                newY -= MathF.Cos(playerAngle) * moveSpeed;
            }

            // Turning logic
            if (Raylib.IsKeyDown(KeyboardKey.Q)) playerAngle -= turnSpeed;
            if (Raylib.IsKeyDown(KeyboardKey.E)) playerAngle += turnSpeed;

            // Update position if valid
            if (map[(int)newX, (int)newY] == 0)
            {
                playerX = newX;
                playerY = newY;
            }

            // Shooting input
            if (Raylib.IsKeyPressed(KeyboardKey.Space))
            {
                if (ammoCount > 0)
                {
                    ammoCount--; // Decrease ammo
                    fireFrames = 3; // Set firing animation frames
                    AddMuzzleFlash();
                    AddParticles();
                    Console.WriteLine("Bang! Ammo left: " + ammoCount);
                }
                else if (!isReloading)
                {
                    isReloading = true;
                    reloadFrames = 60; // Set reloading animation frames
                }
            }
        }

        // Adjust screen height for the raycasting view
        static int screenHeightForRays = screenHeight - 150;  // 100 pixels for roof and floor combined

        // Raycasting function with adjusted screen height and FOV
        static void CastRays()
        {
            int numberOfRays = screenWidth; // Use full screen width for rays
            float fov = MathF.PI / 3; // Adjust the field of view (e.g., PI/3 for 60 degrees)

            for (int rayIndex = 0; rayIndex < numberOfRays; rayIndex++)
            {
                float rayAngle = playerAngle - fov / 2 + (fov * rayIndex / numberOfRays);

                float distanceToWall = 0.0f;
                bool hitWall = false;
                float eyeX = MathF.Cos(rayAngle);
                float eyeY = MathF.Sin(rayAngle);

                // Raycasting loop
                while (!hitWall && distanceToWall < 16)
                {
                    distanceToWall += 0.1f;
                    int testX = (int)(playerX + eyeX * distanceToWall);
                    int testY = (int)(playerY + eyeY * distanceToWall);
                    if (testX < 0 || testX >= mapWidth || testY < 0 || testY >= mapHeight || map[testX, testY] == 1)
                    {
                        hitWall = true;

                        // Calculate the height of the wall slice based on the ray distance
                        float wallHeight = screenHeightForRays / (distanceToWall * MathF.Cos(rayAngle - playerAngle));

                        // Center the wall slice vertically and draw it
                        int wallY = (int)((screenHeightForRays - wallHeight) / 2);
                        Raylib.DrawRectangle(rayIndex, wallY, 1, (int)wallHeight, Color.DarkGreen); // Adjust color as needed
                    }
                }
            }
        }

        // Render minimap function
        static void RenderMinimap()
        {
            const int minimapSize = 250;  // Size of the minimap (250px by 250px for widescreen)
            const int tileSize = 10;  // Size of each tile in the minimap

            // Draw the minimap background
            Raylib.DrawRectangle(0, 0, minimapSize, minimapSize, Color.Black);

            // Render the map in the minimap area
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (map[x, y] == 1)
                    {
                        Raylib.DrawRectangle(x * tileSize, y * tileSize, tileSize, tileSize, Color.Gray); // Wall in grey
                    }
                    else
                    {
                        Raylib.DrawRectangle(x * tileSize, y * tileSize, tileSize, tileSize, Color.LightGray); // Empty space in light grey
                    }
                }
            }

            // Draw player in minimap (as a red dot)
            Raylib.DrawCircle((int)(playerX * tileSize), (int)(playerY * tileSize), 3, Color.Red);
        }

        static void DrawAmmoCounter()
        {
            // Draw the ammo count underneath the minimap (y-position adjusted)
            Raylib.DrawText($"Ammo: {ammoCount}", 10, 260, 20, Color.White); // Placed below the minimap
        }

        // Draw the shotgun in the lower half of the screen
        static void DrawShotgun()
        {
            // Draw the main body of the shotgun
            Raylib.DrawRectangle(screenWidth / 2 - 50, screenHeight - 100, 100, 50, Color.Brown);

            // Draw the barrels, larger and closer together
            Raylib.DrawRectangle(screenWidth / 2 - 40, screenHeight - 150, 30, 50, Color.Black);
            Raylib.DrawRectangle(screenWidth / 2 + 10, screenHeight - 150, 30, 50, Color.Black);

            // Draw the trigger guard
            Raylib.DrawRectangle(screenWidth / 2 - 30, screenHeight - 80, 60, 20, Color.LightGray);

            // Draw the trigger
            Raylib.DrawRectangle(screenWidth / 2 - 10, screenHeight - 70, 20, 10, Color.DarkBrown);

            // Draw the stock
            Raylib.DrawRectangle(screenWidth / 2 - 50, screenHeight - 50, 100, 50, Color.Brown);

            // Handle firing animation
            if (fireFrames > 0)
            {
                fireFrames--;
            }

            // Handle reloading animation
            if (isReloading)
            {
                if (reloadFrames > 0)
                {
                    reloadFrames--;
                    Raylib.DrawText("RELOADING...", screenWidth / 2 - 50, screenHeight - 120, 20, Color.Red);
                }
                else
                {
                    isReloading = false;
                    ammoCount = 10; // Reload ammo
                }
            }
        }

        static void AddMuzzleFlash()
        {
            // Draw a temporary muzzle flash in front of the barrels
            Raylib.DrawCircle(screenWidth / 2, screenHeight - 170, 15, Color.Yellow);
        }

        static void AddParticles()
        {
            // Add particles at the impact point (for simplicity, assume center of screen for now)
            particles.Add((screenWidth / 2, screenHeight / 2, 15));
        }

        static void DrawParticles()
        {
            // Draw and update particles
            for (int i = 0; i < particles.Count; i++)
            {
                var (x, y, frames) = particles[i];

                // Decrease frames over time
                if (frames > 0)
                {
                    Raylib.DrawCircle((int)x, (int)y, 5, Color.LightGray);
                    particles[i] = (x, y, frames - 1);
                }
                else
                {
                    particles.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
