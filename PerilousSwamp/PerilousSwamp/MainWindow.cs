#nullable enable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using PerilousSwamp.MapClasses;

namespace PerilousSwamp;

public partial class MainWindow : Form
{
    public new static Font Font;
    private int _mouseX;
    private int _mouseY;
    private string _currentDecorationImage;
    private readonly TextOutput _textOutput;
    private readonly Map _map;
    private GuiState _guiState;

    static MainWindow()
    {
        Font = new Font();
    }

    public MainWindow()
    {
        _currentDecorationImage = "swamp.png";
        _textOutput = new TextOutput();
        _map = new Map();
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

        DrawMap(118, 8, graphics);
        _textOutput.Draw(graphics);
        e.Graphics.DrawImage(bitmap, pictureBox1.ClientRectangle);
    }

    private void DrawMap(int x, int y, Graphics g)
    {
        for (var visibleY = 0; visibleY < Map.ViewportSize; visibleY++)
        {
            for (var visibleX = 0; visibleX < Map.ViewportSize; visibleX++)
            {
                // Räkna ut exakt vilken koordinat på den STORA kartan vi är på just nu
                var currentMapX = _map.ViewportOffsetX + visibleX;
                var currentMapY = _map.ViewportOffsetY + visibleY;

                // Räkna ut var på skärmen (i pixlar) rutan ska ritas
                var physicalX = x + visibleX * 10;
                var physicalY = y + visibleY * 10;

                // Säkerhetskontroll så vi inte går utanför arrayens gränser (0 till Size-1)
                if (currentMapX >= 0 && currentMapX < Map.Size && currentMapY >= 0 && currentMapY < Map.Size)
                {
                    // VIKTIGT: Arrayen är [y, x] -> alltså [rad, kolumn]
                    var data = _map.Grid[currentMapY, currentMapX];

                    // Rita ut terrängsiffran (1, 2, 3...)
                    g.DrawString(data.ToString(), base.Font, Brushes.Red, physicalX, physicalY);

                    // Kontrollera om spelaren står på denna specifika kartkoordinat
                    if (currentMapX == _map.PlayerX && currentMapY == _map.PlayerY)
                    {
                        g.DrawString("P", base.Font, Brushes.Yellow, physicalX, physicalY);
                    }

                    // Kontrollera om prinsessan står på denna specifika kartkoordinat
                    if (currentMapX == _map.PrincessX && currentMapY == _map.PrincessY)
                    {
                        g.DrawString("Q", base.Font, Brushes.Yellow, physicalX, physicalY);
                    }
                }
            }
        }
    }
}