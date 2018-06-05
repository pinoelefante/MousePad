using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MousePad.Controllers.Mouse
{
    public class MouseHighlighter : Form, IDeviceStatus
    {
        private static readonly Color TRANSPARENT_COLOR = Color.Magenta;
        private static readonly Color HIGHLIGHT_COLOR = Color.Yellow;
        private SolidBrush MouseBrush;

        private MouseOperations mouse;
        private int EllipseRadius = 25;
        public MouseHighlighter() : base()
        {
            MouseBrush = new SolidBrush(HIGHLIGHT_COLOR);
            mouse = new MouseOperations();
            InitializeComponent();
            ChangeRadius(20);
        }
        private const int WS_EX_TRANSPARENT = 0x20;
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle = cp.ExStyle | WS_EX_TRANSPARENT;
                return cp;
            }
        }
        public void DrawHighlight(bool force = false)
        {
            var lastPos = mouse.lastPoint;
            var curPos = mouse.GetCursorPosition();
            if (mouse.Compare(lastPos, curPos) == 0 && !force)
                return;
            using (Graphics g = Graphics.FromHwnd(this.Handle))
            {
                g.Clear(TRANSPARENT_COLOR);
                g.FillEllipse(MouseBrush, curPos.X - EllipseRadius, curPos.Y - EllipseRadius, EllipseRadius * 2, EllipseRadius * 2);
            }
        }
        public void ChangeRadius(int newRadius)
        {
            EllipseRadius = newRadius;
            DrawHighlight(true);
        }

        public bool IsEnabled { get; set; }
        public Action OnEnableAction { get; set; }
        public Action OnDisableAction { get; set; }

        public void DeviceEnabled()
        {
            this.Show();
            IsEnabled = true;
            OnEnableAction?.Invoke();
        }

        public void DeviceDisable()
        {
            this.Hide();
            IsEnabled = false;
            OnDisableAction?.Invoke();
        }

        public void ToggleEnable()
        {
            if (IsEnabled)
                DeviceDisable();
            else
                DeviceEnabled();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.BackColor = TRANSPARENT_COLOR;
            this.TransparencyKey = TRANSPARENT_COLOR;
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MinimizeBox = false;
            this.Name = "MouseHighlighter";
            this.Opacity = 0.4D;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = FormStartPosition.Manual;
            this.TopMost = true;
            this.WindowState = FormWindowState.Maximized;

            this.ResumeLayout(false);
        }
    }
}
