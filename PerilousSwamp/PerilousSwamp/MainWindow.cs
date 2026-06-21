#nullable enable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PerilousSwamp;

public partial class MainWindow : Form
{
    public new static Font Font;
    private TextOutput _textOutput;
    private GuiState _guiState;

    static MainWindow()
    {
        Font = new Font();
    }

    public MainWindow()
    {
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
    }

    private void LockGui(bool locked)
    {
        if (locked)
        {
            Cursor = Cursors.WaitCursor;
        }
        else
        {
            Cursor = Cursors.Default;
        }
    }

    private void pictureBox1_Paint(object sender, PaintEventArgs e)
    {
        e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
        e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        e.Graphics.SmoothingMode = SmoothingMode.None;
        using var bitmap = new Bitmap(Properties.Resources.gui_outline);
        using var graphics = Graphics.FromImage(bitmap);
        _textOutput.Draw(graphics);
        e.Graphics.DrawImage(bitmap, pictureBox1.ClientRectangle);
    }
}