using System.Drawing;
using System.Windows.Forms;

namespace PerilousSwamp;

public class WindowManagement
{
    public static int MouseX { get; private set; }
    public static int MouseY { get; private set; }

    static WindowManagement()
    {
        MouseX = 0;
        MouseY = 0;
    }

    public static void HandleWindowResize(Size clientSize, PictureBox playField)
    {
        const float imageAspect = 1.6f;
        var clientAspect = (float)clientSize.Width / clientSize.Height;

        int newWidth, newHeight;

        if (imageAspect > clientAspect)
        {
            newWidth = clientSize.Width;
            newHeight = (int)(newWidth / imageAspect);
        }
        else
        {
            newHeight = clientSize.Height;
            newWidth = (int)(newHeight * imageAspect);
        }

        playField.Size = new Size(newWidth, newHeight);
        playField.Location = new Point((clientSize.Width - newWidth) / 2, (clientSize.Height - newHeight) / 2);
        playField.Invalidate();
    }

    public static void HandleMouseMove(PictureBox playField, MouseEventArgs e)
    {
        var scaleX = 320.0 / playField.Width;
        var scaleY = 200.0 / playField.Height;
        MouseX = (int)(e.X * scaleX);
        MouseY = (int)(e.Y * scaleY);
    }

    public static void HandleKeyPress(PictureBox playField, GuiState guiState, GameState gameState, NumberInput numberInput, KeyPressEventArgs keyPressEventArgs)
    {
        if (guiState == GuiState.WaitingForUserInput)
        {
            if (gameState == GameState.EnterCombatPoints)
            {
                if (numberInput == null)
                    return;

                switch (keyPressEventArgs.KeyChar)
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
                        numberInput.EnterDigit(keyPressEventArgs.KeyChar);
                        playField.Refresh();
                        break;
                }
            }
        }
    }
}