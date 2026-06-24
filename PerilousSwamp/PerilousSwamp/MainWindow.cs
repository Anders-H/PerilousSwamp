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
    private Bitmap? _gameBitmap;
    private int _mouseX;
    private int _mouseY;
    private string _currentDecorationImage;
    private readonly TextOutput _textOutput;
    private readonly Map _map;
    private GuiState _guiState;
    private GameState _gameState;

    static MainWindow()
    {
        Font = new Font();
    }

    public MainWindow()
    {
        _gameState = GameState.PickDirection;
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
#if !DEBUG
        TypeWrite("In this game, you find yourself in a swampy forest. Your task is to find your way to the edge, alive, and with as much treasure as possible.");
        TypeWrite("");
        _currentDecorationImage = "princess.png";
        TypeWrite("A beautiful princess is held by an evil wizard. The king wouldn't mind if you could release her...");
        _currentDecorationImage = "evil_wizard.png";
        TypeWrite("");
#endif
        TypeWrite("Should you have to leave early, typing \"out\" should get you out - permanently.");
        _currentDecorationImage = "swamp.png";
        LockGui(false);
        Refresh();
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
        _gameBitmap ??= new Bitmap(320, 200, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using var g = Graphics.FromImage(_gameBitmap);
        g.DrawImage(Properties.Resources.gui_outline, 0, 0, 320, 200);
        using var graphics = Graphics.FromImage(_gameBitmap);
        var aktuellDekor = imgListDekor.Images[_currentDecorationImage];

        if (aktuellDekor != null)
            graphics.DrawImage(aktuellDekor, new Point(8, 8));

        DrawMap(118, 8, graphics);
        _textOutput.Draw(graphics);

        if (_guiState == GuiState.WaitingForUserInput)
        {
            switch (_gameState)
            {
                case GameState.PickDirection:
                    graphics.DrawImage(Properties.Resources.Compass, new Rectangle(214, 3, 110, 110));
                    break;
            }
        }

        e.Graphics.DrawImage(_gameBitmap, pictureBox1.ClientRectangle);
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

    private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
    {
        if (pictureBox1 == null)
            return;

        var scaleX = 320.0 / pictureBox1.Width;
        var scaleY = 200.0 / pictureBox1.Height;
        _mouseX = (int)(e.X * scaleX);
        _mouseY = (int)(e.Y * scaleY);

#if DEBUG
        Text = $@"{_mouseX} x {_mouseY}";
#endif
    }

    private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
    {
        if (_guiState == GuiState.WaitingForUserInput)
        {
            switch (_gameState)
            {
                case GameState.PickDirection:
                    switch (Compass.GetDirectionFromCoordinate(_mouseX, _mouseY))
                    {
                        case CompassDirection.NoOperation:
                            TypeWrite("Nop");
                            break;
                        case CompassDirection.North:
                            TypeWrite("North");
                            break;
                        case CompassDirection.Ne:
                            TypeWrite("Northeast");
                            break;
                        case CompassDirection.East:
                            TypeWrite("East");
                            break;
                        case CompassDirection.Se:
                            TypeWrite("Southeast");
                            break;
                        case CompassDirection.South:
                            TypeWrite("South");
                            break;
                        case CompassDirection.Sw:
                            TypeWrite("Southwest");
                            break;
                        case CompassDirection.West:
                            TypeWrite("West");
                            break;
                        case CompassDirection.Nw:
                            TypeWrite("Northwest");
                            break;
                    }
                    break;
            }
        }
    }
}