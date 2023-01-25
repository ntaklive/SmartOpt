using System;
using System.Runtime.InteropServices;

namespace SmartOpt;

public class Win32
{
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    const int SW_HIDE = 0;
    const int SW_SHOW = 5;

    public static void HideConsole()
    {
        IntPtr handle = GetConsoleWindow();
        ShowWindow(handle, SW_HIDE);
    }
}