﻿using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MouseRecorder
{
    class MouseHook
    {
        private int hookId;
        public WinUserDll.HookProc hookProc;

        private const int WH_MOUSE_LL = 14;
        private const int WM_MOUSEMOVE      = 0x0200;
        private const int WM_LBUTTONDOWN    = 0x0201;
        private const int WM_LBUTTONUP      = 0x0202;
        private const int WM_RBUTTONDOWN    = 0x0204;
        private const int WM_RBUTTONUP      = 0x0205;
        private const int WM_MOUSEWHEEL     = 0x020A;

        public struct POINT
        {
            private int x;
            private int y;
        }

        private struct MSLLHOOKSTRUC
        {
            public POINT pt;
            private uint mouseData;
            private uint flags;
            private uint time;
            private IntPtr dwExtraInfo;
        }

        public void SetHook()
        {
            hookProc = new WinUserDll.HookProc(MouseHookProc);
            hookId = WinUserDll.SetWindowsHookEx(WH_MOUSE_LL, hookProc, IntPtr.Zero, 0);
        }

        public void UnHook()
        {
            WinUserDll.UnhookWindowsHookEx(hookId);
        }

        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_MOUSEMOVE)
                {
                    if (MouseMoveEvent != null) MouseMoveEvent(this, ((MSLLHOOKSTRUC)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUC))).pt);
                }
                else
                {
                    switch ((Int32)wParam)
                    {
                        case WM_LBUTTONDOWN:
                            if (MouseDownEvent != null) MouseDownEvent(this, MouseButtons.Left);
                            break;

                        case WM_LBUTTONUP:
                            if (MouseUpEvent != null) MouseUpEvent(this, MouseButtons.Left);
                            break;

                        case WM_RBUTTONDOWN:
                            if (MouseDownEvent != null) MouseDownEvent(this, MouseButtons.Right);
                            break;

                        case WM_RBUTTONUP:
                            if (MouseUpEvent != null) MouseUpEvent(this, MouseButtons.Right);
                            break;
                    }
                }
            }

            return WinUserDll.CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public delegate void MousePointHandler(object sender, POINT pt);
        public delegate void MouseHandler(object sender, MouseButtons key);
        public event MousePointHandler MouseMoveEvent;
        public event MouseHandler MouseDownEvent;
        public event MouseHandler MouseUpEvent;
    }
}
