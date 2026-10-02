using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace DeskAnim;

internal static class Native
{
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_TRANSPARENT = 0x00000020;
    private const int WS_EX_TOOLWINDOW = 0x00000080;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    public static void SetClickThrough(Window w, bool enable)
    {
        var h = new WindowInteropHelper(w).Handle;
        if (h == IntPtr.Zero) return;
        int style = GetWindowLong(h, GWL_EXSTYLE);
        style = enable ? style | WS_EX_TRANSPARENT : style & ~WS_EX_TRANSPARENT;
        SetWindowLong(h, GWL_EXSTYLE, style);
    }

    public static void HideFromAltTab(Window w)
    {
        var h = new WindowInteropHelper(w).Handle;
        if (h == IntPtr.Zero) return;
        int style = GetWindowLong(h, GWL_EXSTYLE);
        SetWindowLong(h, GWL_EXSTYLE, style | WS_EX_TOOLWINDOW);
    }
}
