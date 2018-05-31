using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace MousePad
{
    public class MouseOperations : IComparer<POINT>
    {
        private const uint MOUSEEVENTF_MOVE = 0x01;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x02;
        private const uint MOUSEEVENTF_LEFTUP = 0x04;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const uint MOUSEEVENTF_RIGHTUP = 0x10;

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);
        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        public Action OnMouseMoveAction { get; set; }

        public POINT lastPoint = new POINT() { X = 0, Y = 0 };
        public POINT GetCursorPosition()
        {
            if(GetCursorPos(out POINT newPoint) && (newPoint.X!=lastPoint.X || newPoint.Y!=lastPoint.Y))
            {
                lastPoint = newPoint;
                return newPoint;
            }
            return lastPoint;
        }
        protected bool SetCursorPosition(int x, int y)
        {
            bool r = SetCursorPos(x, y);
            if (r)
                OnMouseMoveAction?.Invoke();
            return r;
        }
        protected void LeftButtonDown()
        {
            var p = GetCursorPosition();
            mouse_event(MOUSEEVENTF_LEFTDOWN, p.X, p.Y, 0, UIntPtr.Zero);
        }
        protected void LeftButtonUp()
        {
            var p = GetCursorPosition();
            mouse_event(MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, UIntPtr.Zero);
        }
        protected void RightButtonDown()
        {
            var p = GetCursorPosition();
            mouse_event(MOUSEEVENTF_RIGHTDOWN, p.X, p.Y, 0, UIntPtr.Zero);
        }
        protected void RightButtonUp()
        {
            var p = GetCursorPosition();
            mouse_event(MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, UIntPtr.Zero);
        }
        protected void LeftButtonClick()
        {
            var p = GetCursorPosition();
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, p.X, p.Y, 0, UIntPtr.Zero);
        }
        protected void RightButtonClick()
        {
            var p = GetCursorPosition();
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, p.X, p.Y, 0, UIntPtr.Zero);
        }
        protected void LeftButtonDoubleClick()
        {
            LeftButtonClick();
            Thread.Sleep(100);
            LeftButtonClick();
        }
        protected void RightButtonDoubleClick()
        {
            RightButtonClick();
            Thread.Sleep(100);
            RightButtonClick();
        }
        protected void Move(int xSpeed, int ySpeed)
        {
            var pos = GetCursorPosition();
            var newX = pos.X + (5 * xSpeed);
            var newY = pos.X + (5 * ySpeed);
            mouse_event(MOUSEEVENTF_MOVE, newX, newY, 0, UIntPtr.Zero);
        }

        public int Compare(POINT last, POINT newPoint)
        {
            if (last.X == newPoint.X && last.Y == newPoint.Y)
                return 0;
            return 1;
        }
    }
}
