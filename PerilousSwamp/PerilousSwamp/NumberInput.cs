using System;
using System.Drawing;
using System.Globalization;

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

    public void EnterDigit(char digit)
    {
        if (Characters.Length > 4)
            Characters = Characters.Substring(0, 4);

        Characters += digit;
        CursorPosition = Characters.Length;
    }

    public void Backspace()
    {
        if (Characters.Length <= 0)
            return;

        Characters = Characters.Substring(0, Characters.Length - 1);
        CursorPosition = Characters.Length;
    }

    public int GetNumber()
    {
        try
        {
            var parsed = int.Parse(Characters, NumberStyles.Any);
            return parsed;
        }
        catch
        {
            return 0;
        }
    }

    public void Clear()
    {
        Characters = "";
        CursorPosition = 0;
    }

    public void Draw(Graphics g, int x, int y)
    {
        if (Characters.Length > 0)
        {
            var currentX = x;

            foreach (var character in Characters)
            {
                g.DrawImage(_font.GetCharacter(character), currentX, y);
                currentX += 8;
            }
        }

        g.DrawImage(_font.GetCharacter('§'), x + CursorPosition * 8, y);

    }
}