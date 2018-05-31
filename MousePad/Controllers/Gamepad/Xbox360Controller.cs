using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace MousePad.Controllers.Gamepad
{
    public class Xbox360Controller : MouseOperations, IDeviceStatus
    {
        private static List<Controller> controllers = new List<Controller>() { new Controller(UserIndex.One), new Controller(UserIndex.Two), new Controller(UserIndex.Three), new Controller(UserIndex.Four) };
        public int Speed { get; set; } = 1;
        public int PixelMovement { get; set; } = 5;
        public bool IsEnabled { get; set; } = true;
        public Action OnEnableAction { get; set; }
        public Action OnDisableAction { get; set; }
        private InputSimulator input;

        public Xbox360Controller()
        {
            input = new InputSimulator();
        }
        private State prevState = new State() { PacketNumber = -1 };
        //private Keystroke lastKeystrokeDown = ;
        public void GetControllerInput()
        {
            var controller = FirstAvailable();
            if (controller == null)
                return;
            var state = controller.GetState();
            if(state.PacketNumber != prevState.PacketNumber)
            {
                prevState = state;
                controller.GetKeystroke(DeviceQueryType.Gamepad, out Keystroke keystroke);
                
                switch(keystroke.VirtualKey)
                {
                    case GamepadKeyCode.A:
                    case GamepadKeyCode.LeftThumbPress:
                        ManageKeyUpDown(keystroke, () => LeftButtonUp(), () => LeftButtonDown());
                        break;
                    case GamepadKeyCode.X:
                    case GamepadKeyCode.RightThumbPress:
                        ManageKeyUpDown(keystroke, () => RightButtonUp(), () => RightButtonDown());
                        break;
                    case GamepadKeyCode.LeftThumbLeft:
                        ManageAxis(controller, state, -Speed, 0);
                        break;
                    case GamepadKeyCode.LeftThumbRight:
                        ManageAxis(controller, state, Speed, 0);
                        break;
                    case GamepadKeyCode.LeftThumbUp:
                        ManageAxis(controller, state, 0, -Speed);
                        break;
                    case GamepadKeyCode.LeftThumbDown:
                        ManageAxis(controller, state, 0, Speed);
                        break;
                    case GamepadKeyCode.LeftThumbDownright:
                        ManageAxis(controller, state, Speed, Speed);
                        break;
                    case GamepadKeyCode.LeftThumbUpright:
                        ManageAxis(controller, state, Speed, -Speed);
                        break;
                    case GamepadKeyCode.RightThumbDownLeft:
                        ManageAxis(controller, state, -Speed, Speed);
                        break;
                    case GamepadKeyCode.RightThumbUpLeft:
                        ManageAxis(controller, state, -Speed, -Speed);
                        break;
                    case GamepadKeyCode.RightShoulder:
                        ManageSpeed(1, keystroke);
                        break;
                    case GamepadKeyCode.LeftShoulder:
                        ManageSpeed(-1, keystroke);
                        break;
                    case GamepadKeyCode.Back:
                    case GamepadKeyCode.Start:
                        if (keystroke.Flags.HasFlag(KeyStrokeFlags.KeyDown))
                            ManageStartSelect(state);
                        else if (keystroke.Flags.HasFlag(KeyStrokeFlags.KeyUp))
                        {
                            if (IsStartSelect)
                            {
                                IsStartSelect = !IsStartSelect;
                                ToggleEnable();
                            }
                            else
                            {
                                switch (keystroke.VirtualKey)
                                {
                                    case GamepadKeyCode.Back:
                                        ManageKeyUpDown(keystroke, () => input.Keyboard.KeyPress(VirtualKeyCode.BACK), null);
                                        break;
                                    case GamepadKeyCode.Start:
                                        ManageKeyUpDown(keystroke, () => input.Keyboard.KeyPress(VirtualKeyCode.LWIN), null);
                                        break;
                                }
                            }
                        }
                        break;
                    case GamepadKeyCode.None:
                        break;
                    default:
                        Console.WriteLine($"Pressed: {keystroke.VirtualKey} Flag: {keystroke.Flags}");
                        break;
                }
            }
        }
        private bool IsStartSelect = false;
        private void ManageKeyUpDown(Keystroke keystroke, Action up, Action down)
        {
            if (!IsEnabled)
                return;
            if (keystroke.Flags.HasFlag(KeyStrokeFlags.KeyDown))
                down?.Invoke();
            else if (keystroke.Flags.HasFlag(KeyStrokeFlags.KeyUp))
                up?.Invoke();
            else
            {
                Debug.WriteLine("Azione non definita");
            }
        }
        private void ManageStartSelect(State state)
        {
            var startDown = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Start);
            var selectDown = state.Gamepad.Buttons.HasFlag(GamepadButtonFlags.Back);
            IsStartSelect = startDown & selectDown;
        }
        private void ManageSpeed(int x, Keystroke keystroke)
        {
            if (!IsEnabled)
                return;
            var lastSpeed = Speed;
            if(keystroke.Flags.HasFlag(KeyStrokeFlags.KeyUp))
            {
                if (x < 0 && Speed > 1)
                    Speed -= 1;
                else if (x > 0 && Speed < 5)
                    Speed += 1;
                if(Speed!=lastSpeed)
                    Debug.WriteLine($"Speed = {Speed}");
            }
        }
        private void ManageAxis(Controller controller, State lastState, int x, int y)
        {
            if (!IsEnabled)
                return;
            var repeat = true;
            while(repeat)
            {
                ManageAxis(x, y);
                var newState = controller.GetState();
                if(newState.PacketNumber != lastState.PacketNumber)
                {
                    controller.GetKeystroke(DeviceQueryType.Gamepad, out Keystroke newKeystroke);
                    if (newKeystroke.Flags.HasFlag(KeyStrokeFlags.KeyUp))
                        repeat = false;
                }
                Thread.Sleep(20);
            }
        }
        private void ManageAxis(int x, int y)
        {
            var pos = GetCursorPosition();
            var newX = pos.X + (PixelMovement * x);
            var newY = pos.Y + (PixelMovement * y);
            SetCursorPosition(newX, newY);
        }
        public Controller FirstAvailable()
        {
            foreach(var conn in controllers)
            {
                if (conn.IsConnected)
                    return conn;
            }
            return null;
        }
        private void SetDeadzone()
        {
            foreach(var c in controllers)
            {
            }
        }
        private Vibration disableVibration = new Vibration()
        {
            LeftMotorSpeed = 0,
            RightMotorSpeed = 0
        };
        public void Vibrate(ushort power, int milliseconds)
        {
            var controller = FirstAvailable();

            ushort powerx = (ushort)((ushort.MaxValue / 100) * power);
            controller.SetVibration(new Vibration()
            {
                LeftMotorSpeed = powerx,
                RightMotorSpeed = powerx
            });
            Thread.Sleep(milliseconds);
            controller.SetVibration(disableVibration);
        }
        public void DeviceEnabled()
        {
            IsEnabled = true;
            OnEnableAction?.Invoke();
            var controller = FirstAvailable();
            if (controller == null)
                return;
            Vibrate(100, 1000);
        }
        public void DeviceDisable()
        {
            IsEnabled = false;
            OnDisableAction?.Invoke();
            var controller = FirstAvailable();
            if (controller == null)
                return;
            Vibrate(20, 1000);
        }

        public void ToggleEnable()
        {
            if (IsEnabled)
                DeviceDisable();
            else
                DeviceEnabled();
        }
    }
}
