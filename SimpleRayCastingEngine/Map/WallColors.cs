using Raylib_cs;  // Add this to use Color
using System;

namespace SimpleRayCastingEngine.Map
{
    public static class WallColors
    {
        // Method to return color based on the wall type
        public static Color GetColor(WallType type)
        {
            switch (type)
            {
                case WallType.Wall:
                    return Color.DarkGray; // Standard gray wall
                case WallType.Water:
                    return Color.Blue;     // Water should be blue
                case WallType.Wood:
                    return Color.Brown;    // Wood should be brown
                default:
                    return Color.Black;    // Default to black for Empty or unknown types
            }
        }
    }
}
