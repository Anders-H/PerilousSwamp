using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PerilousSwamp.Input
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowsMessage
    {
        public IntPtr handle;
        public uint msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point p;
    }
}