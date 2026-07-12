using System.Drawing;

namespace PerilousSwamp;

public static class Extensions
{
    public static bool HitTest(this Rectangle me, int x, int y)
    {
        var x2 = me.X + me.Width;
        var y2 = me.Y + me.Height;

        if (x < me.X || x >= x2)
            return false;

        if (y >= me.Y && y < y2)
            return true;

        return false;
    }
}