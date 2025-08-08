namespace MarbleScroll.Core
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using MarbleScroll.Configuration;
    using MarbleScroll.Interop;

    public class MarbleScrollService
    {
        private IntPtr hookID = IntPtr.Zero;
        private readonly WindowsApi.LowLevelMouseProc hookCallback;
        private readonly ScrollProcessor scrollProcessor = new();
        private bool isScrolling = false;

        public MarbleScrollService()
        {
            hookCallback = HookCallback;
        }

        public void Start()
        {
            hookID = SetHook(hookCallback);
        }

        public void Stop()
        {
            WindowsApi.UnhookWindowsHookEx(hookID);
        }

        private static IntPtr SetHook(WindowsApi.LowLevelMouseProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return WindowsApi.SetWindowsHookEx(WindowsApi.WH_MOUSE_LL, proc, WindowsApi.GetModuleHandle(curModule.ModuleName), 0);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode >= 0)
                {
                    var type = (MouseMessages)wParam;

                    // Fast path for most common case - regular mouse moves when not scrolling
                    if (type == MouseMessages.WM_MOUSEMOVE && !isScrolling)
                    {
                        return WindowsApi.CallNextHookEx(hookID, nCode, wParam, lParam);
                    }

                    // Only parse expensive structures when we actually need them
                    return type switch
                    {
                        MouseMessages.WM_XBUTTONDOWN => HandlePotentialXButtonDown(nCode, wParam, lParam),
                        MouseMessages.WM_XBUTTONUP => HandlePotentialXButtonUp(nCode, wParam, lParam),
                        MouseMessages.WM_MOUSEMOVE when isScrolling => HandleMouseMoveWhileScrolling(lParam),
                        _ => WindowsApi.CallNextHookEx(hookID, nCode, wParam, lParam)
                    };
                }

                return WindowsApi.CallNextHookEx(hookID, nCode, wParam, lParam);
            }
            catch
            {
                return WindowsApi.CallNextHookEx(hookID, nCode, wParam, lParam);
            }
        }

        private static bool IsTargetXButton(uint xButton)
        {
            return xButton is ScrollConfiguration.BackButton or ScrollConfiguration.ForwardButton;
        }

        private IntPtr HandlePotentialXButtonDown(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            var xButton = WindowsApi.GetXButton(hookStruct.mouseData);
            
            if (IsTargetXButton(xButton))
            {
                isScrolling = true;
                scrollProcessor.InitializeScroll(hookStruct);
                
                // Focus window under mouse pointer for scrolling
                FocusWindowUnderMouse(hookStruct);
                
                return new IntPtr(1);
            }
            
            return WindowsApi.CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        private IntPtr HandlePotentialXButtonUp(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            var xButton = WindowsApi.GetXButton(hookStruct.mouseData);
            
            if (IsTargetXButton(xButton) && isScrolling)
            {
                isScrolling = false;
                return scrollProcessor.ShouldSuppressClick() ? new IntPtr(1) : WindowsApi.CallNextHookEx(hookID, nCode, wParam, lParam);
            }
            
            return WindowsApi.CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        private IntPtr HandleMouseMoveWhileScrolling(IntPtr lParam)
        {
            var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            scrollProcessor.ProcessMouseMove(hookStruct);
            return new IntPtr(1);
        }

        private void FocusWindowUnderMouse(MSLLHOOKSTRUCT hookStruct)
        {
            var point = new POINT { x = hookStruct.pt.x, y = hookStruct.pt.y };
            IntPtr focusWindow = WindowsApi.WindowFromPoint(point);
            IntPtr foregroundWindow = WindowsApi.GetForegroundWindow();
            
            // Only focus if window is not already focused
            if (WindowsApi.GetAncestor(foregroundWindow, 3) != WindowsApi.GetAncestor(focusWindow, 3))
                WindowsApi.SetForegroundWindow(focusWindow);
        }
    }
}