namespace MarbleScroll.Interop
{
    internal enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_BACKBUTTONDOWN = 0x020B,
        WM_BACKBUTTONUP = 0x020C,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_XBUTTONDOWN = 0x020B,
        WM_XBUTTONUP = 0x020C
    }

    internal enum MouseEvents
    {
        WHEEL = 0x0800,
        HWHEEL = 0x1000,
        MOVE = 0x0001,
        ABSMOVE = 0x8001,
        MIDDLEDOWN = 0x0020,
        MIDDLEUP = 0x0040
    }
}