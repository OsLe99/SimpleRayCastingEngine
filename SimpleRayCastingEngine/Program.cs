using Raylib_cs;
using SimpleRayCastingEngine;
using SimpleRayCastingEngine.GameObjects;

namespace RaycastingExample
{
    class Program
    {
        static void Main()
        {
            Raylib.InitWindow(1280, 720, "Raycasting Example");
            Raylib.SetTargetFPS(60);

            Game game = new Game();

            while (!Raylib.WindowShouldClose())
            {
                game.Update();
                game.Render();
            }

            Raylib.CloseWindow();
        }
    }
}