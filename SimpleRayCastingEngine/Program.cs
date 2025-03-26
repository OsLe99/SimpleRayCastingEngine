using System;
using Raylib_cs;
using System.Numerics;

namespace RaycastingExample
{
    class Program
    {
        // Define screen dimensions
        const int screenWidth = 800;
        const int screenHeight = 600;

        // Map settings
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

        // Player settings
        static float playerX = 2.0f;
        static float playerY = 2.0f;
        static float playerAngle = 0.0f;
        static float moveSpeed = 0.1f;
        static float turnSpeed = 0.05f;

        // Shader settings
        static Shader wallShader;
        static Vector3 wallColor = new Vector3(1.0f, 0.0f, 0.0f); // Red color

        static void Main(string[] args)
        {
            // Initialize Raylib
            Raylib.InitWindow(screenWidth, screenHeight, "Raycasting Example");
            Raylib.SetTargetFPS(60);

            // Load shaders
            string vertexShaderFilePath = @"C:\Users\Oscar\source\repos\SimpleRayCastingEngine\SimpleRayCastingEngine\Shaders\shader.vs";
            string fragmentShaderFilePath = @"C:\Users\Oscar\source\repos\SimpleRayCastingEngine\SimpleRayCastingEngine\Shaders\shader.fs";
            Console.WriteLine($"Vertex shader file path: {vertexShaderFilePath}");
            Console.WriteLine($"Fragment shader file path: {fragmentShaderFilePath}");

            wallShader = Raylib.LoadShader(vertexShaderFilePath, fragmentShaderFilePath);

            // Main game loop
            while (!Raylib.WindowShouldClose())
            {
                // Update player movement
                HandleInput();

                // Draw everything
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.DarkGray);

                // Set shader and pass uniform values
                int wallColorLocation = Raylib.GetShaderLocation(wallShader, "wallColor");
                if (wallColorLocation != -1)
                {
                    Raylib.SetShaderValue(wallShader, wallColorLocation, wallColor, ShaderUniformDataType.Vec3);
                    Console.WriteLine("Uniform location found and set.");
                }
                else
                {
                    Console.WriteLine("Uniform location not found.");
                }

                // Apply the shader while rendering the map
                Raylib.BeginShaderMode(wallShader);
                Console.WriteLine("Shader activated.");
                RenderMap();
                Raylib.EndShaderMode();
                Console.WriteLine("Shader deactivated.");

                // Cast rays
                CastRays();

                Raylib.EndDrawing();
            }

            // Unload shader when done
            Raylib.UnloadShader(wallShader);

            // Close window
            Raylib.CloseWindow();
        }

        static void HandleInput()
        {
            if (Raylib.IsKeyDown(KeyboardKey.W))
            {
                playerX += MathF.Cos(playerAngle) * moveSpeed;
                playerY += MathF.Sin(playerAngle) * moveSpeed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.S))
            {
                playerX -= MathF.Cos(playerAngle) * moveSpeed;
                playerY -= MathF.Sin(playerAngle) * moveSpeed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.A))
            {
                playerAngle -= turnSpeed;
            }
            if (Raylib.IsKeyDown(KeyboardKey.D))
            {
                playerAngle += turnSpeed;
            }
        }

        static void RenderMap()
        {
            // Draw the map as a grid
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    if (map[x, y] == 1)
                    {
                        Raylib.DrawRectangle(x * 50, y * 50, 50, 50, Color.DarkBrown); // Wall
                    }
                    else
                    {
                        Raylib.DrawRectangle(x * 50, y * 50, 50, 50, Color.LightGray); // Empty space
                    }
                }
            }
        }

        static void CastRays()
        {
            // Cast rays to simulate the view from the player
            for (int x = 0; x < screenWidth; x++)
            {
                float rayAngle = playerAngle + ((x - screenWidth / 2.0f) / screenWidth) * MathF.PI / 3.0f;
                float distanceToWall = 0.0f;
                bool hitWall = false;

                float eyeX = MathF.Cos(rayAngle);
                float eyeY = MathF.Sin(rayAngle);

                // Cast rays until they hit a wall or go out of bounds
                while (!hitWall && distanceToWall < 16)
                {
                    distanceToWall += 0.1f;
                    int testX = (int)(playerX + eyeX * distanceToWall);
                    int testY = (int)(playerY + eyeY * distanceToWall);

                    if (testX < 0 || testX >= mapWidth || testY < 0 || testY >= mapHeight)
                    {
                        hitWall = true;
                        distanceToWall = 16;
                    }
                    else if (map[testX, testY] == 1)
                    {
                        hitWall = true;
                    }
                }

                // Color walls based on distance
                int ceiling = (int)(screenHeight / 2.0f - screenHeight / distanceToWall);
                int floorHeight = screenHeight - ceiling;

                Raylib.DrawLine(x, 0, x, ceiling, Color.DarkBlue); // Ceiling
                Raylib.DrawLine(x, ceiling, x, floorHeight, Color.DarkGreen); // Wall
                Raylib.DrawLine(x, floorHeight, x, screenHeight, Color.DarkGray); // Floor
            }
        }
    }
}
