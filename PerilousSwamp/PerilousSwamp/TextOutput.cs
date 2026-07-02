#nullable enable
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PerilousSwamp;

public class TextOutput
{
    private readonly string?[] _lines;
    private readonly PictureBox _pictureBox;

    public TextOutput(PictureBox pictureBox)
    {
        _lines = new string?[10];
        _pictureBox = pictureBox;
    }

    public void TypeWrite(string text1, string text2)
    {
        TypeWrite(text1);
        TypeWrite(text2);
    }
    
    public void TypeWrite(string text1, string text2, string text3)
    {
        TypeWrite(text1);
        TypeWrite(text2);
        TypeWrite(text3);
    }

    public void TypeWrite(string text)
    {
        const int typeWriterSpeed = 30;

        if (string.IsNullOrWhiteSpace(text))
        {
            SetText("");
            _pictureBox.Invalidate();
            Application.DoEvents();
            Thread.Sleep(typeWriterSpeed);
            return;
        }

        var lines = WordWrap(text);
        foreach (var line in lines)
        {
            SetText("");

            if (string.IsNullOrWhiteSpace(line))
            {
                _pictureBox.Invalidate();
                Application.DoEvents();
                Thread.Sleep(typeWriterSpeed);
            }

            var l = line.Trim();

            for (var i = 0; i < l.Length; i++)
            {
                AssignLastRow(l.Substring(0, i + 1));
                _pictureBox.Invalidate();
                Application.DoEvents();
                Thread.Sleep(typeWriterSpeed);
            }
        }
    }

    public void SetText(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            ScrollUp();
            return;
        }

        var wrapped = WordWrapper.WordWrap(38, text);
        var lines = wrapped.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

        foreach (var line in lines)
        {
            if (line == lines.Last() && string.IsNullOrWhiteSpace(line))
                return;

            ScrollUp();
            _lines[9] = line;
        }
    }

    public void AssignLastRow(string newText) =>
        _lines[9] = newText;

    private void ScrollUp()
    {
        for (var i = 0; i < _lines.Length - 1; i++)
            _lines[i] = _lines[i + 1];

        _lines[9] = "";
    }

    public void Draw(Graphics graphics)
    {
        const int xStart = 8;
        const int yStart = 114;

        for (var i = 0; i < _lines.Length; i++)
        {
            var line = _lines[i] ?? "";

            for (var c = 0; c < line.Length; c++)
                graphics.DrawImage(MainWindow.Font.GetCharacter(line[c]), new Point(xStart + c * 8, yStart + i * 8));
        }
    }

    public string[] WordWrap(string text)
    {
        var result = WordWrapper.WordWrap(38, text).Split(["\r\n", "\r", "\n"], StringSplitOptions.None).ToList();
        
        if (string.IsNullOrWhiteSpace(result.Last()))
            result.RemoveAt(result.Count - 1);

        return result.ToArray();
    }
        
}

file static class WordWrapper
{
    public static string WordWrap(int columnCount, string text)
    {
        static int Break(string breakText, int breakPos, int max)
        {
            var position = max;

            while (position >= 0 && !char.IsWhiteSpace(breakText[breakPos + position]))
                position--;

            if (position < 0)
                return max;

            while (position >= 0 && char.IsWhiteSpace(breakText[breakPos + position]))
                position--;

            return position + 1;
        }

        int wordBreak;
        var s = new StringBuilder();

        for (var charPointer = 0; charPointer < text.Length; charPointer = wordBreak)
        {
            var endOfLine = text.IndexOf("\r\n", charPointer, StringComparison.Ordinal);

            if (endOfLine < 0)
                endOfLine = text.Length;

            wordBreak = endOfLine <= 0 ? text.Length : endOfLine + 2;

            if (endOfLine > charPointer)
            {
                do
                {
                    var length = endOfLine - charPointer;
                    if (length > columnCount)
                        length = Break(text, charPointer, columnCount);
                    s.Append(text, charPointer, length);
                    s.AppendLine();
                    charPointer += length;
                    while (charPointer < endOfLine && char.IsWhiteSpace(text[charPointer]))
                        charPointer++;
                } while (endOfLine > charPointer);

                continue;
            }

            s.AppendLine();
        }

        return s.ToString();
    }
}