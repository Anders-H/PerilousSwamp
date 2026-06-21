using System;
using System.Runtime.InteropServices;

namespace PerilousSwamp.Input;

public class Buffer
{
    [DllImport("User32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool PeekMessage(out WindowsMessage message, IntPtr handle, uint filterMin, uint filterMax, uint flags);

    private static void FlushBuffer(IntPtr handle, uint first, uint last)
    {
        const uint pmRemove = 1;
        do
        {

            if (!PeekMessage(out var windowsMessage, handle, first, last, pmRemove))
                return;

        } while (true);
    }

    public static void FlushKeyboardBuffer(IntPtr handle)
    {
        const uint wmKeyFirst = 256;
        const uint wmKeyLast = 264;

        FlushBuffer(handle, wmKeyFirst, wmKeyLast);
    }

    public static void FlushMouseBuffer(IntPtr handle)
    {
        const uint wmMouseFirst = 512;
        const uint wmMouseLast = 526;

        FlushBuffer(handle, wmMouseFirst, wmMouseLast);
    }
}