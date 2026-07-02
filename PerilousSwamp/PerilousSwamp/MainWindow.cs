#nullable enable
using System;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using MrSwampMonster;
using PerilousSwamp.MapClasses;

namespace PerilousSwamp;

public partial class MainWindow : Form
{
    public new static Font Font;
    private const int MonsterImageOffset = 3;
    private Bitmap? _gameBitmap;
    private int _mouseX;
    private int _mouseY;
    private string _currentDecorationImage;
    private Monster? _currentMonster;
    private TextOutput? _textOutput;
    private Map? _map;
    private MonsterMap? _monsterMap;
    private GameProperties? _gameProperties;
    private NumberInput? _numberInput;
    private GuiState _guiState;
    private GameState _gameState;

    static MainWindow()
    {
        Font = new Font();
    }

    public MainWindow()
    {
        _gameState = GameState.PickDirection;
        _currentMonster = null;
        _currentDecorationImage = "swamp.png";
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
        _textOutput!.TypeWrite("In this game, you find yourself in a swampy forest. Your task is to find your way to the edge, alive, and with as much treasure as possible.", "");
        _currentDecorationImage = "princess.png";
        _textOutput!.TypeWrite("A beautiful princess is held by an evil wizard. The king wouldn't mind if you could release her...");
        _currentDecorationImage = "evil_wizard.png";
        _textOutput!.TypeWrite("", "Should you have to leave early, typing \"out\" should get you out - permanently.");
#endif
        _currentDecorationImage = "swamp.png";
        LockGui(false);
        pictureBox1.Invalidate();
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
        _textOutput = new TextOutput(pictureBox1);
        pictureBox1.Invalidate();
        MainWindow_Resize(sender, e);
        PlayGameIntro();
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

        if (_currentMonster != null)
        {
            switch (_gameState)
            {
                case GameState.RunFightBribe:
                    graphics.DrawImage(imgListDekor.Images[_currentMonster.AliveImageIndex + MonsterImageOffset], new Point(8, 8));
                    break;
                case GameState.EnterCombatPoints:
                    graphics.DrawImage(imgListDekor.Images[_currentMonster.AttackImageIndex + MonsterImageOffset], new Point(8, 8));
                    break;
                default:
                    var aktuellDekor = imgListDekor.Images[_currentDecorationImage];

                    if (aktuellDekor != null)
                        graphics.DrawImage(aktuellDekor, new Point(8, 8));

                    break;
            }
        }
        else
        {
            var aktuellDekor = imgListDekor.Images[_currentDecorationImage];

            if (aktuellDekor != null)
                graphics.DrawImage(aktuellDekor, new Point(8, 8));
        }

        DrawMap(118, 8, graphics);
        _textOutput!.Draw(graphics);

        if (_guiState == GuiState.WaitingForUserInput)
        {
            switch (_gameState)
            {
                case GameState.PickDirection:
                    graphics.DrawImage(Properties.Resources.Compass, new Rectangle(214, 3, 110, 110));
                    break;
                case GameState.RunFightBribe:
                    graphics.DrawImage(Properties.Resources.fight_run_bribe, new Rectangle(225, -5, 86, 115));
                    break;
                case GameState.EnterCombatPoints:
                    _numberInput?.Draw(graphics, 8, 186);
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
                            _textOutput!.TypeWrite("", "North");
                            MovePlayer(-1, 0);
                            break;
                        case CompassDirection.Ne:
                            _textOutput!.TypeWrite("", "Northeast");
                            MovePlayer(-1, 1);
                            break;
                        case CompassDirection.East:
                            _textOutput!.TypeWrite("", "East");
                            MovePlayer(0, 1);
                            break;
                        case CompassDirection.Se:
                            _textOutput!.TypeWrite("", "Southeast");
                            MovePlayer(1, 1);
                            break;
                        case CompassDirection.South:
                            _textOutput!.TypeWrite("", "South");
                            MovePlayer(1, 0);
                            break;
                        case CompassDirection.Sw:
                            _textOutput!.TypeWrite("", "Southwest");
                            MovePlayer(1, -1);
                            break;
                        case CompassDirection.West:
                            _textOutput!.TypeWrite("", "West");
                            MovePlayer(0, -1);
                            break;
                        case CompassDirection.Nw:
                            _textOutput!.TypeWrite("", "Northwest");
                            MovePlayer(-1, -1);
                            break;
                    }
                    break;
                case GameState.RunFightBribe:
                    var fightButton = new Rectangle(241, 9, 25, 16);
                    var runButton = new Rectangle(241, 47, 25, 16);
                    var bribeButton = new Rectangle(241, 86, 25, 16);

                    if (fightButton.HitTest(_mouseX, _mouseX))
                    {
                        _textOutput!.TypeWrite("", "Fight!");
                        Fight();
                    }
                    else if (runButton.HitTest(_mouseX, _mouseX))
                    {
                        _textOutput!.TypeWrite("", "Run!");
                        Run();
                    }
                    else if (bribeButton.HitTest(_mouseX, _mouseX))
                    {
                        _textOutput!.TypeWrite("", "Bribe!");
                        Bribe();
                    }
                    break;
                case GameState.EnterCombatPoints:
                    break;
            }

            LockGui(false);
        }
    }

    private void Fight()
    {
        _textOutput!.TypeWrite("", "How many combat points? Type a number and press Enter.", "");
        _numberInput = new NumberInput(Font);
        _guiState = GuiState.WaitingForUserInput;
        _gameState = GameState.EnterCombatPoints;
        pictureBox1.Refresh();
    }

    private void Run()
    {

    }

    private void Bribe()
    {

    }

    private void MovePlayer(int diffY, int diffX)
    {
        if (_map == null || _gameProperties == null)
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
             _textOutput!.TypeWrite("", "Too wet that way, clot!");
            LockGui(false);
            _gameState = GameState.PickDirection;
            _guiState = GuiState.WaitingForUserInput;
            pictureBox1.Invalidate();
        }
        else if (data == MapGenerator.Free)
        {
            _map.PlayerX = newX;
            _map.PlayerY = newY;
            _map.UpdateViewport();
            pictureBox1.Invalidate();

            var monster = _monsterMap!.GetMonsterFromMapPosition(newX, newY);

            if (monster != null)
            {
                if (monster.IsGone)
                {
                    _textOutput!.TypeWrite("", $"Once there was a monster here, a {monster.MonsterName}. It is long gone now.");
                    _gameState = GameState.PickDirection;
                    _guiState = GuiState.WaitingForUserInput;
                    pictureBox1.Invalidate();
                }
                else if (!monster.IsAlive)
                {
                    _textOutput!.TypeWrite("", $"You are standing by the dead body of a monster. It was once a {monster.MonsterName}.");
                    _gameState = GameState.PickDirection;
                    _guiState = GuiState.WaitingForUserInput;
                    pictureBox1.Invalidate();
                }
                else
                {
                    _currentMonster = monster;
                    _textOutput!.TypeWrite("", $"A {monster.MonsterName} is guarding SOME SHIT. His combat points come to {monster.MonsterCombatStrength}.");
                    // TODO Treasure
                    _textOutput!.TypeWrite("", "Do you wish to fight, run, or bribe?");
                    _gameState = GameState.RunFightBribe;
                    _guiState = GuiState.WaitingForUserInput;
                    pictureBox1.Invalidate();
                }
            }
            else
            {
                _gameState = GameState.PickDirection;
                _guiState = GuiState.WaitingForUserInput;
                pictureBox1.Invalidate();
            }
        }
        else if (data == MapGenerator.Player)
        {
            _map.PlayerX = newX;
            _map.PlayerY = newY;
            _map.UpdateViewport();
            _textOutput!.TypeWrite("", "This is where you found yourself when the game started.");
            pictureBox1.Invalidate();
            _gameState = GameState.PickDirection;
            _guiState = GuiState.WaitingForUserInput;
            pictureBox1.Invalidate();
        }
        else if (data == MapGenerator.Princess)
        {
            if (_gameProperties!.PrincessIsPickedUp)
            {
                _map.PlayerX = newX;
                _map.PlayerY = newY;
                _map.UpdateViewport();
                pictureBox1.Invalidate();
                _gameState = GameState.PickDirection;
                _guiState = GuiState.WaitingForUserInput;
                pictureBox1.Invalidate();
                LockGui(true);
                _textOutput!.TypeWrite("", "You have been here before. You picked up the princess here, and she is happy to be rescued by you! Bring her on your journey ahead!");
                LockGui(false);
            }
            else
            {
                _map.PlayerX = newX;
                _map.PlayerY = newY;
                _map.UpdateViewport();
                pictureBox1.Invalidate();
                _gameState = GameState.PickDirection;
                _guiState = GuiState.WaitingForUserInput;
                pictureBox1.Invalidate();
                LockGui(true);
                
                // A green, dirty wizard is guarding a fair princess.
                // The wizard has his pet Bunyip with him, and his combat points come to 120.

                // Do you wish to fight, run or bribe?

                //--- Sucess:
                _gameProperties.PrincessIsPickedUp = true;
                _textOutput!.TypeWrite("", "You sure smashed that monster. Your ill-gotten gains now come to XXXXX points. The princess comes with you.");

                //--- Failure:

                // Too bad... The monster ate you... And took all your treasure!
                // Pity about the princess... The wizard fed her to a dragon. The king is not all that pleased.

                // Try again? You could get lucky!

                LockGui(false);
                pictureBox1.Invalidate();
            }
        }
    }

    private void MainWindow_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (_guiState == GuiState.WaitingForUserInput)
        {
            if (_gameState == GameState.EnterCombatPoints)
            {
                if (_numberInput == null)
                    return;

                switch (e.KeyChar)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        _numberInput.EnterDigit(e.KeyChar);
                        pictureBox1.Refresh();
                        break;
                }
            }
        }
    }

    private void MainWindow_KeyDown(object sender, KeyEventArgs e)
    {
        if (_guiState == GuiState.WaitingForUserInput)
        {
            if (_gameState == GameState.EnterCombatPoints)
            {
                if (_numberInput == null)
                    return;

                switch (e.KeyCode)
                {
                    case Keys.Back:
                        _numberInput.Backspace();
                        pictureBox1.Refresh();
                        break;
                    case Keys.Enter:
                        var combatPoints = _numberInput.GetNumber();
                        _numberInput.Clear();
                        _gameState = GameState.PickDirection;
                        _guiState = GuiState.WaitingForUserInput;
                        _textOutput!.TypeWrite($"You entered {combatPoints} combat points.");
                        break;
                }
            }
        }
    }
}