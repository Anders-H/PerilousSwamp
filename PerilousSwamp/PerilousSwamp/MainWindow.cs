#nullable enable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;
using MrSwampMonster;
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
    private Map? _map;
    private MonsterMap? _monsterMap;
    private GameProperties? _gameProperties;
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
        NewGame();
        InitializeComponent();
    }

    private void NewGame()
    {
        _gameState = GameState.PickDirection;
        _map = new Map();
        _monsterMap = new MonsterMap();
        _gameProperties = new GameProperties();
    }

    private void PlayGameIntro()
    {
        LockGui(true);
#if !DEBUG
        TypeWrite("In this game, you find yourself in a swampy forest. Your task is to find your way to the edge, alive, and with as much treasure as possible.");
        TypeWrite("");
        _currentDecorationImage = "princess.png";
        TypeWrite("A beautiful princess is held by an evil wizard. The king wouldn't mind if you could release her...");
        _currentDecorationImage = "evil_wizard.png";
        TypeWrite("");
        TypeWrite("Should you have to leave early, typing \"out\" should get you out - permanently.");
#endif
        _currentDecorationImage = "swamp.png";
        LockGui(false);
        Refresh();
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
        PlayGameIntro();
    }

    private void TypeWrite(string text)
    {
        const int typeWriterSpeed = 30;

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
                case GameState.RunFightBribe:
                    graphics.DrawImage(Properties.Resources.fight_run_bribe, new Rectangle(214, 3, 110, 110));
                    break;
            }
        }

        e.Graphics.DrawImage(_gameBitmap, pictureBox1.ClientRectangle);
    }

    private void DrawMap(int x, int y, Graphics g)
    {
        if (_map == null || _gameProperties == null)
            return;

        for (var visibleY = 0; visibleY < Map.ViewportSize; visibleY++)
        {
            for (var visibleX = 0; visibleX < Map.ViewportSize; visibleX++)
            {
                var currentMapX = _map.ViewportOffsetX + visibleX;
                var currentMapY = _map.ViewportOffsetY + visibleY;
                var physicalX = x + visibleX * 10;
                var physicalY = y + visibleY * 10;

                if (currentMapX is >= 0 and < Map.Size && currentMapY is >= 0 and < Map.Size)
                {
                    var data = _map.Grid[currentMapY, currentMapX];

                    switch (data)
                    {
                        case MapGenerator.Obstacle:
                            g.DrawImage(imgListMap.Images[3], new Rectangle(physicalX, physicalY, 10, 10));
                            break;
                        case MapGenerator.Edge:
                            g.DrawImage(imgListMap.Images[0], new Rectangle(physicalX, physicalY, 10, 10));
                            break;
                        case MapGenerator.Free:
                        case MapGenerator.Player:
                        case MapGenerator.Princess:
                            // Draw nothing for free space. Player and princess is just their start position, not current.
                            break;
                    }

                    if (currentMapX == _map.PlayerX && currentMapY == _map.PlayerY)
                        g.DrawImage(imgListMap.Images[1], new Rectangle(physicalX, physicalY, 10, 10));

                    if (!_gameProperties.PrincessIsPickedUp && currentMapX == _map.PrincessX && currentMapY == _map.PrincessY)
                        g.DrawImage(imgListMap.Images[2], new Rectangle(physicalX, physicalY, 10, 10));
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
            LockGui(true);

            switch (_gameState)
            {
                case GameState.PickDirection:
                    switch (Compass.GetDirectionFromCoordinate(_mouseX, _mouseY))
                    {
                        case CompassDirection.NoOperation:
                            break;
                        case CompassDirection.North:
                            TypeWrite("");
                            TypeWrite("North");
                            MovePlayer(-1, 0);
                            break;
                        case CompassDirection.Ne:
                            TypeWrite("");
                            TypeWrite("Northeast");
                            MovePlayer(-1, 1);
                            break;
                        case CompassDirection.East:
                            TypeWrite("");
                            TypeWrite("East");
                            MovePlayer(0, 1);
                            break;
                        case CompassDirection.Se:
                            TypeWrite("");
                            TypeWrite("Southeast");
                            MovePlayer(1, 1);
                            break;
                        case CompassDirection.South:
                            TypeWrite("");
                            TypeWrite("South");
                            MovePlayer(1, 0);
                            break;
                        case CompassDirection.Sw:
                            TypeWrite("");
                            TypeWrite("Southwest");
                            MovePlayer(1, -1);
                            break;
                        case CompassDirection.West:
                            TypeWrite("");
                            TypeWrite("West");
                            MovePlayer(0, -1);
                            break;
                        case CompassDirection.Nw:
                            TypeWrite("");
                            TypeWrite("Northwest");
                            MovePlayer(-1, -1);
                            break;
                    }
                    break;
            }

            LockGui(false);
        }
    }

    private void MovePlayer(int diffY, int diffX)
    {
        if (_map == null)
            return;

        var newX = _map.PlayerX + diffX;
        var newY = _map.PlayerY + diffY;
        var data = _map.Grid[newY, newX];

        if (data == MapGenerator.Edge)
        {

        }
        else if (data == MapGenerator.Obstacle)
        {
            LockGui(true);
            TypeWrite("");
            TypeWrite("Too wet that way, clot!");
            LockGui(false);
            _gameState = GameState.PickDirection;
            _guiState = GuiState.WaitingForUserInput;
            Refresh();
        }
        else if (data == MapGenerator.Free)
        {
            _map.PlayerX = newX;
            _map.PlayerY = newY;
            _map.UpdateViewport();
            Refresh();

            var monster = _monsterMap!.GetMonsterFromMapPosition(newX, newY);

            if (monster != null)
            {
                if (monster.IsGone)
                {
                    TypeWrite("");
                    TypeWrite($"Once there was a monster here, a {monster.MonsterName}. It is long gone now.");
                    _gameState = GameState.PickDirection;
                    _guiState = GuiState.WaitingForUserInput;
                    Refresh();

                }
                else if (!monster.IsAlive)
                {
                    TypeWrite("");
                    TypeWrite($"You are standing by the dead body of a monster. It was once a {monster.MonsterName}.");
                    _gameState = GameState.PickDirection;
                    _guiState = GuiState.WaitingForUserInput;
                    Refresh();
                }
                else
                {
                    TypeWrite("");
                    TypeWrite($"Your combat strength is {_gameProperties.PlayerCombatStrength}. A {monster.MonsterName} says hi.");
                    TypeWrite("");
                    TypeWrite($"A {monster.MonsterName}");
                }
            }
            else
            {
                _gameState = GameState.PickDirection;
                _guiState = GuiState.WaitingForUserInput;
                Refresh();
            }
        }
        else if (data == MapGenerator.Player)
        {
            _map.PlayerX = newX;
            _map.PlayerY = newY;
            _map.UpdateViewport();
            TypeWrite("");
            TypeWrite("This is where you found yourself when the game started.");
            Refresh();
            _gameState = GameState.PickDirection;
            _guiState = GuiState.WaitingForUserInput;
            Refresh();
        }
        else if (data == MapGenerator.Princess)
        {
            if (_gameProperties!.PrincessIsPickedUp)
            {
                _map.PlayerX = newX;
                _map.PlayerY = newY;
                _map.UpdateViewport();
                Refresh();
                _gameState = GameState.PickDirection;
                _guiState = GuiState.WaitingForUserInput;
                Refresh();
                LockGui(true);
                TypeWrite("");
                TypeWrite("You have been here before. You picked up the princess here, and she is happy to be rescued by you! Bring her on your journey ahead!");
                LockGui(false);
            }
            else
            {
                _map.PlayerX = newX;
                _map.PlayerY = newY;
                _map.UpdateViewport();
                Refresh();
                _gameState = GameState.PickDirection;
                _guiState = GuiState.WaitingForUserInput;
                Refresh();
                LockGui(true);
                TypeWrite("");
                
                // A green, dirty wizard is guarding a fair princess.
                // The wizard has his pet Bunyip with him, and his combat points come to 120.

                // Do you wish to fight, run or bribe?

                //--- Sucess:
                _gameProperties.PrincessIsPickedUp = true;
                TypeWrite("");
                TypeWrite("You sure smashed that monster. Your ill-gotten gains now come to XXXXX points. The princess comes with you.");

                //--- Failure:

                // Too bad... The monster ate you... And took all your treasure!
                // Pity about the princess... The wizard fed her to a dragon. The king is not all that pleased.

                // Try again? You could get lucky!

                LockGui(false);
                Refresh();
            }
        }
    }
}