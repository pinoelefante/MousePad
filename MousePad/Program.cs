using MousePad.Controllers.Gamepad;
using MousePad.Controllers.Mouse;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MousePad
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            MouseHighlighter highlighter = new MouseHighlighter();
            highlighter.DeviceEnabled();
            Xbox360Controller x360 = new Xbox360Controller();
            x360.OnMouseMoveAction = () => highlighter.DrawHighlight();
            x360.OnEnableAction = () => highlighter.DeviceEnabled();
            x360.OnDisableAction = () => highlighter.DeviceDisable();
            while (true)
            {
                highlighter.DrawHighlight();
                x360.GetControllerInput();
                Thread.Sleep(10);
            }
        }
    }
}
