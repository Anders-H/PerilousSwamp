using System.Drawing;

namespace PerilousSwamp;

public static class Compass
{
    public static CompassDirection GetDirectionFromCoordinate(int x, int y)
    {
        if (new Rectangle(256, 13, 23, 15).Contains(new Point(x, y)))
            return CompassDirection.North;

        if (new Rectangle(283, 20, 24, 19).Contains(new Point(x, y)))
            return CompassDirection.Ne;

        if (new Rectangle(296, 49, 17, 21).Contains(new Point(x, y)))
            return CompassDirection.East;

        if (new Rectangle(283, 76, 24, 19).Contains(new Point(x, y)))
            return CompassDirection.Se;

        if (new Rectangle(256, 86, 23, 15).Contains(new Point(x, y)))
            return CompassDirection.South;

        if (new Rectangle(230, 74, 24, 19).Contains(new Point(x, y)))
            return CompassDirection.Sw;

        if (new Rectangle(224, 49, 17, 21).Contains(new Point(x, y)))
            return CompassDirection.West;

        if (new Rectangle(230, 20, 24, 19).Contains(new Point(x, y)))
            return CompassDirection.Nw;

        return CompassDirection.NoOperation;
    }
}