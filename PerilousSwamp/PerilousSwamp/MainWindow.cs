#nullable enable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

namespace PerilousSwamp;

public partial class MainWindow : Form
{
    public new static Font Font;
    private string _currentDecorationImage;
    private readonly TextOutput _textOutput;
    private GuiState _guiState;

    static MainWindow()
    {
        Font = new Font();
    }

    public MainWindow()
    {
        _currentDecorationImage = "swamp.png";
        _textOutput = new TextOutput();
        InitializeComponent();
    }

    private void MainWindow_Resize(object sender, EventArgs e)
    {
        const float imageAspect = 1.6f;
        var clientAspect = (float)ClientSize.Width / ClientSize.Height;

        int newWidth, newHeight;

        if (imageAspect > clientAspect)
        {
            newWidth = ClientSize.Width;
            newHeight = (int)(newWidth / imageAspect);
        }
        else
        {
            newHeight = ClientSize.Height;
            newWidth = (int)(newHeight * imageAspect);
        }

        pictureBox1.Size = new Size(newWidth, newHeight);
        pictureBox1.Location = new Point((ClientSize.Width - newWidth) / 2, (ClientSize.Height - newHeight) / 2);
        pictureBox1.Invalidate();
    }

    private void MainWindow_Shown(object sender, EventArgs e)
    {
        Refresh();
        MainWindow_Resize(sender, e);
        LockGui(true);
        TypeWrite("In this game, you find yourself in a swampy forest. Your task is to find your way to the edge, alive, and with as much treasure as possible.");
        TypeWrite("");
        _currentDecorationImage = "princess.png";
        TypeWrite("A beautiful princess is held by an evil wizard. The king wouldn't mind if you could release her...");
        _currentDecorationImage = "evil_wizard.png";
        TypeWrite("");
        TypeWrite("Should you have to leave early, typing \"out\" should get you out - permanently.");
        _currentDecorationImage = "swamp.png";
        Refresh();
        LockGui(false);
    }

    private void TypeWrite(string text)
    {
        const int typeWriterSpeed = 80;

        if (string.IsNullOrWhiteSpace(text))
        {
            _textOutput.SetText("");
            pictureBox1.Invalidate();
            Application.DoEvents();
            Thread.Sleep(typeWriterSpeed);
            return;
        }

        var lines = _textOutput.WordWrap(text);
        foreach (var line in lines)
        {
            _textOutput.SetText("");

            if (string.IsNullOrWhiteSpace(line))
            {
                pictureBox1.Invalidate();
                Application.DoEvents();
                Thread.Sleep(typeWriterSpeed);
            }

            var l = line.Trim();

            for (var i = 0; i < l.Length; i++)
            {
                _textOutput.AssignLastRow(l.Substring(0, i + 1));
                pictureBox1.Invalidate();
                Application.DoEvents();
                Thread.Sleep(typeWriterSpeed);
            }
        }
    }

    private void LockGui(bool locked)
    {
        try
        {
            if (locked)
            {
                _guiState = GuiState.DeliveringOutput;
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                Input.Buffer.FlushKeyboardBuffer(Handle);
                Input.Buffer.FlushMouseBuffer(Handle);
                _guiState = GuiState.WaitingForUserInput;
                Cursor = Cursors.Default;
            }
        }
        catch
        {
            // ignored
        }
    }

    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.SmoothingMode = SmoothingMode.None;
        using var bitmap = new Bitmap(Properties.Resources.gui_outline);
        using var graphics = Graphics.FromImage(bitmap);
        var aktuellDekor = imgListDekor.Images[_currentDecorationImage];

        if (aktuellDekor != null)
            graphics.DrawImage(aktuellDekor, new Point(8, 8));

        _textOutput.Draw(graphics);
        e.Graphics.DrawImage(bitmap, pictureBox1.ClientRectangle);
    }
}