using System.Drawing;

namespace PerilousSwamp;

public class NumberInput
{
    private readonly Font _font;
    public int CursorPosition { get; private set; }
    public string Characters { get; private set; }

    public NumberInput(Font font)
    {
        _font = font;
        CursorPosition = 0;
        Characters = "";
    }

    public void Draw(Graphics g, int x, int y)
    {

    }
}