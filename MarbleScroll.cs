using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MarbleScroll
{
    public class MarbleScroll
    {
        public bool FocusWindow { get; set; }
        private IntPtr hookID = IntPtr.Zero;
        private LowLevelMouseProc hookCallback;

        private const int SENSITIVITY_X = 200;
        private const int DISTANCE_X = 200;
        private const int SENSITIVITY_Y = 50;
        private const int DISTANCE_Y = 200;

        private bool isScroll = false;
        private bool supressButtonClick = false;

        private int startX;
        private int startY;
        private int dx;
        private int dy;

        private const uint backButton = 1;
        private const uint forwardButton = 2;

        public MarbleScroll()
        {
            hookCallback = HookCallback;
        }

        public void Start()
        {
            hookID = SetHook(hookCallback);
        }

        public void Stop()
        {
            UnhookWindowsHookEx(hookID);
        }

        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private uint GetXButton(uint hookStruct)
        {
            return (hookStruct & 0xFFFF0000) >> 16;
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            MouseMessages type = (MouseMessages)wParam;
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            uint xButton = GetXButton(hookStruct.mouseData);

            if (IsXButtonDown(type, xButton))
            {
                return HandleXButtonDown(hookStruct);
            }
            else if (IsXButtonUp(type, xButton))
            {
                return HandleXButtonUp();
            }
            else if (IsScrolling(type))
            {
                return HandleMouseMove(hookStruct);
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        private bool IsXButtonDown(MouseMessages type, uint xButton)
        {
            return type == MouseMessages.WM_XBUTTONDOWN && (xButton == backButton || xButton == forwardButton);
        }

        private bool IsXButtonUp(MouseMessages type, uint xButton)
        {
            return type == MouseMessages.WM_XBUTTONUP && (xButton == backButton || xButton == forwardButton);
        }

        private bool IsScrolling(MouseMessages type)
        {
            return isScroll && type == MouseMessages.WM_MOUSEMOVE;
        }

        private IntPtr HandleXButtonDown(MSLLHOOKSTRUCT hookStruct)
        {
            InitializeScrollState(hookStruct);
            return new IntPtr(1);
        }

        private void InitializeScrollState(MSLLHOOKSTRUCT hookStruct)
        {
            isScroll = true;
            supressButtonClick = false;
            startX = hookStruct.pt.x;
            startY = hookStruct.pt.y;
            dx = 0;
            dy = 0;
        }

        private IntPtr HandleXButtonUp()
        {
            isScroll = false;
            if (supressButtonClick)
            {
                return new IntPtr(1);
            }
            return IntPtr.Zero;
        }

        private IntPtr HandleMouseMove(MSLLHOOKSTRUCT hookStruct)
        {
            UpdateScrollDeltas(hookStruct);
            ProcessHorizontalScroll();
            ProcessVerticalScroll();
            return new IntPtr(1);
        }

        private void UpdateScrollDeltas(MSLLHOOKSTRUCT hookStruct)
        {
            dx += hookStruct.pt.x - startX;
            dy += hookStruct.pt.y - startY;
        }

        private void ProcessHorizontalScroll()
        {
            if (Math.Abs(dx) > SENSITIVITY_X)
            {
                int scrollDirection = CalculateHorizontalScrollDirection();
                dy = 0;
                ExecuteHorizontalScroll(scrollDirection);
                supressButtonClick = true;
            }
        }

        private int CalculateHorizontalScrollDirection()
        {
            int direction = DISTANCE_X;
            if (dx < 0)
            {
                direction *= -1;
                dx += SENSITIVITY_X;
            }
            else
            {
                dx -= SENSITIVITY_X;
            }
            return direction;
        }

        private void ExecuteHorizontalScroll(int direction)
        {
            Task.Factory.StartNew(() =>
            {
                mouse_event((uint)MouseEvents.HWHEEL, 0U, 0U, direction, UIntPtr.Zero);
            });
        }

        private void ProcessVerticalScroll()
        {
            if (Math.Abs(dy) > SENSITIVITY_Y)
            {
                int scrollDirection = CalculateVerticalScrollDirection();
                ExecuteVerticalScroll(scrollDirection);
                supressButtonClick = true;
            }
        }

        private int CalculateVerticalScrollDirection()
        {
            int direction = DISTANCE_Y;
            if (dy > 0)
            {
                direction *= -1;
                dy -= SENSITIVITY_Y;
            }
            else
            {
                dy += SENSITIVITY_Y;
            }
            return direction;
        }

        private void ExecuteVerticalScroll(int direction)
        {
            Task.Factory.StartNew(() =>
            {
                mouse_event((uint)MouseEvents.WHEEL, 0U, 0U, direction, UIntPtr.Zero);
            });
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, UIntPtr dwExtraInfo);

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private const int WH_MOUSE_LL = 14;

        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205,
            WM_XBUTTONDOWN = 0x020B,
            WM_XBUTTONUP = 0x020C,
            WM_XBUTTONDBLCLK = 0x020D
        }

        private enum MouseEvents
        {
            ABSOLUTE = 0x8000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            MOVE = 0x0001,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            XDOWN = 0x0080,
            XUP = 0x0100,
            WHEEL = 0x0800,
            HWHEEL = 0x1000
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
    }
}
