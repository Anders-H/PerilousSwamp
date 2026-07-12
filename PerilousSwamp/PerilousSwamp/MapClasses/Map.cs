using System;

namespace PerilousSwamp.MapClasses;

public class Map
{
    public const int Size = 20;
    public const int ViewportSize = 10;
    public int[,] Grid { get; }
    public int PlayerX { get; set; }
    public int PlayerY { get; set; }
    public int PrincessX { get; private set; }
    public int PrincessY { get; private set; }
    public int ViewportOffsetX { get; private set; }
    public int ViewportOffsetY { get; private set; }

    public Map()
    {
        Grid = MapGenerator.GenerateMap(this);
        UpdateViewport();
    }

    public bool IsPositionFree(int x, int y)
    {
        if (x < 0 || x >= Size || y < 0 || y >= Size)
            return false;
        
        return Grid[y, x] != MapGenerator.Obstacle && Grid[y, x] != MapGenerator.Edge;
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

    public void UpdateViewport()
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
                var playerHasMargin = ContainsWithMargin(ox, oy, PlayerX, PlayerY, margin: 1);

                if (!playerHasMargin)
                {
                    var cameraIsLocked = ox is 0 or maxOffsetX && oy is 0 or maxOffsetY;

                    if (!cameraIsLocked || !Contains(ox, oy, PlayerX, PlayerY))
                        continue;
                }

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

    private static bool ContainsWithMargin(int ox, int oy, int x, int y, int margin) =>
        x >= ox + margin && x < ox + ViewportSize - margin &&
        y >= oy + margin && y < oy + ViewportSize - margin;

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