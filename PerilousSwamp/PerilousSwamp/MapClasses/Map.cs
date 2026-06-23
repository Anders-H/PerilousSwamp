using System;

namespace PerilousSwamp.MapClasses;

public class Map
{
    public const int Size = 20;
    public const int ViewportSize = 10;
    public int[,] Grid { get; private set; }
    public int PlayerX { get; private set; }
    public int PlayerY { get; private set; }
    public int PrincessX { get; private set; }
    public int PrincessY { get; private set; }
    public int ViewportOffsetX { get; private set; }
    public int ViewportOffsetY { get; private set; }

    public Map()
    {
        Grid = MapGenerator.GenerateMap(this);
        UpdateViewport();
    }

    public void SetPlayerPosition(int x, int y)
    {
        PlayerX = x;
        PlayerY = y;
    }

    public void SetPrincessPosition(int x, int y)
    {
        PrincessX = x;
        PrincessY = y;
    }

    private void UpdateViewport()
    {
        var bestX = 0;
        var bestY = 0;
        var bestScore = int.MinValue;
        const int maxOffsetX = Size - ViewportSize;
        const int maxOffsetY = Size - ViewportSize;

        for (var oy = 0; oy <= maxOffsetY; oy++)
        {
            for (var ox = 0; ox <= maxOffsetX; ox++)
            {
                var playerVisible = Contains(ox, oy, PlayerX, PlayerY);

                if (!playerVisible)
                    continue;

                var score = ScoreViewport(ox, oy);

                if (score <= bestScore)
                    continue;

                bestScore = score;
                bestX = ox;
                bestY = oy;
            }
        }

        ViewportOffsetX = bestX;
        ViewportOffsetY = bestY;
    }

    private int ScoreViewport(int ox, int oy)
    {
        var viewCenterXi = ox + ViewportSize / 2;
        var viewCenterYi = oy + ViewportSize / 2;
        var distToPlayer = Math.Abs(PlayerX - viewCenterXi) + Math.Abs(PlayerY - viewCenterYi);

        if (Contains(ox, oy, PrincessX, PrincessY))
            return 1000 - distToPlayer;

        var distX = DistanceToRange(PrincessX, ox, ox + ViewportSize - 1);
        var distY = DistanceToRange(PrincessY, oy, oy + ViewportSize - 1);
        var manhattanToEdge = distX + distY;
        return -manhattanToEdge * 10 - distToPlayer;
    }

    private static bool Contains(int ox, int oy, int x, int y) =>
        x >= ox && x < ox + ViewportSize && y >= oy && y < oy + ViewportSize;

    private static int DistanceToRange(int value, int min, int max)
    {
        if (value < min)
            return min - value;
        
        if (value > max)
            return value - max;
        
        return 0;
    }
}