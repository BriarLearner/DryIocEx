using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;

namespace DryIocEx.Core.Util;

public interface IUtilSingle : IUtil
{
    /// <summary>
    ///     窗体程序检测,会自动将旧的窗体移动到最前面
    /// </summary>
    /// <param name="processname"></param>
    /// <param name="exit"></param>
    /// <returns></returns>
    IDisposable CheckSingleton(string processname, out bool exit);
}

[Util]
public class SingleUtil : IUtilSingle
{
    public enum EShowWindowCommands
    {
        Hide = 0,
        Normal,
        ShowMinimized,
        ShowMaximized,
        ShowNoActivate,
        Show,
        Minimize,
        ShowMinNoActive,
        ShowNA,
        Restore,
        ShowDefault,
        ForceMinimize
    }

    public IDisposable CheckSingleton(string processname, out bool exit)
    {
        throw new NotImplementedException();
    }

    private static bool IsMaxmimized(IntPtr hwnd)
    {
        throw new NotImplementedException();
    }

    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public EShowWindowCommands showCmd;
        public Point ptMinPosition;
        public Point ptMaxPosition;
        public Rectangle rcNormalPosition;
    }

    #region Win32 API functions

    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsZoomed(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool FlashWindow(IntPtr hWnd, bool bInvert);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

    #endregion
}