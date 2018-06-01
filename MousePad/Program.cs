using MousePad.Controllers;
using MousePad.Controllers.Gamepad;
using MousePad.Controllers.Mouse;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MousePad
{
    class Program
    {
        private static MouseHighlighter highlighter;
        private static Xbox360Controller x360;
        private static ApplicationsMonitor monitor;
        static void Main(string[] args)
        {
            var basedir = AppDomain.CurrentDomain.BaseDirectory;
            Directory.SetCurrentDirectory(basedir);

            HideConsole();
            CreateTrayIcon();
            Console.CursorVisible = true;

            highlighter = new MouseHighlighter();
            highlighter.DeviceEnabled();
            highlighter.ChangeRadius(20);

            x360 = new Xbox360Controller
            {
                OnMouseMoveAction = () => highlighter.DrawHighlight(),
                OnEnableAction = () => highlighter.DeviceEnabled(),
                OnDisableAction = () => highlighter.DeviceDisable()
            };
            /*
            monitor = new ApplicationsMonitor();
            monitor.AddDevice(highlighter);
            monitor.AddDevice(x360);
            */
            while (!exit)
            {
                highlighter.DrawHighlight();
                // x360.GetControllerInput();
                Thread.Sleep(10);
            }
        }
        private static bool exit = false;
        private static NotifyIcon notifyIcon1;
        private static ContextMenu contextMenu1;
        private static MenuItem menuItem1;
        private static IContainer components;
        private static void CreateTrayIcon()
        {
            components = new Container();
            contextMenu1 = new ContextMenu();
            menuItem1 = new MenuItem
            {

                // Initialize menuItem1
                Index = 0,
                Text = "E&xit"
            };
            menuItem1.Click += (s, e) => { exit = true; };

            // Initialize contextMenu1
            contextMenu1.MenuItems.AddRange(
                        new MenuItem[] { menuItem1 });

            // Create the NotifyIcon.
            notifyIcon1 = new NotifyIcon(components)
            {

                // The Icon property sets the icon that will appear
                // in the systray for this application.)

                Icon = new Icon("controller.ico"),

                // The ContextMenu property sets the menu that will
                // appear when the systray icon is right clicked.
                ContextMenu = contextMenu1,

                // The Text property sets the text that will be displayed,
                // in a tooltip, when the mouse hovers over the systray icon.
                Text = "MousePad",
                Visible = true
            };


            notifyIcon1.Click += (s, e) => 
            {
                
            };
        }
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        public static void HideConsole()
        {
            var wndptr = GetConsoleWindow();
            ShowWindow(wndptr, SW_HIDE);
        }
    }
}
