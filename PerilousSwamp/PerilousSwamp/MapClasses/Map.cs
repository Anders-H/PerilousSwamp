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
        // Försök hitta ett offset som visar både spelaren och prinsessan.
        // Annars prioriteras spelaren och vi minimerar avståndet till prinsessan.
        int bestX = 0, bestY = 0;
        int bestScore = int.MinValue;

        int maxOffsetX = Size - ViewportSize;
        int maxOffsetY = Size - ViewportSize;

        for (int oy = 0; oy <= maxOffsetY; oy++)
        {
            for (int ox = 0; ox <= maxOffsetX; ox++)
            {
                bool playerVisible = Contains(ox, oy, PlayerX, PlayerY);
                if (!playerVisible)
                    continue;

                int score = ScoreViewport(ox, oy);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestX = ox;
                    bestY = oy;
                }
            }
        }

        ViewportOffsetX = bestX;
        ViewportOffsetY = bestY;
    }

    private int ScoreViewport(int ox, int oy)
    {
        // Prinsessan synlig = högt baspoäng
        if (Contains(ox, oy, PrincessX, PrincessY))
            return 1000;

        // Annars: ju närmre prinsessan är viewportens kant, desto bättre
        int distX = DistanceToRange(PrincessX, ox, ox + ViewportSize - 1);
        int distY = DistanceToRange(PrincessY, oy, oy + ViewportSize - 1);
        int manhattanToEdge = distX + distY;

        return -manhattanToEdge;
    }

    private static bool Contains(int ox, int oy, int x, int y)
    {
        return x >= ox && x < ox + ViewportSize &&
               y >= oy && y < oy + ViewportSize;
    }

    private static int DistanceToRange(int value, int min, int max)
    {
        if (value < min) return min - value;
        if (value > max) return value - max;
        return 0;
    }
}