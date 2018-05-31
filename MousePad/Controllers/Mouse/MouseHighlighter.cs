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
        private MouseOperations mouse;
        private int EllipseRadius = 25;
        public MouseHighlighter() : base()
        {
            mouse = new MouseOperations();
            InitializeComponent();
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
            // Debug.WriteLine($"X:{curPos.X} Y:{curPos.Y}");
            this.Top = curPos.Y-EllipseRadius;
            this.Left = curPos.X-EllipseRadius;
        }
        public void ChangeRadius(int newRadius)
        {
            EllipseRadius = newRadius;
            this.Size = new Size(EllipseRadius * 2, EllipseRadius * 2);
            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, EllipseRadius * 2, EllipseRadius * 2);
            this.Region = new Region(gp);
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

            this.TopMost = true;
            this.Opacity = 0.4;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.MinimumSize = new Size(30, 30);
            this.MaximumSize = new Size(150, 150);
            this.BackColor = Color.Yellow;
            this.StartPosition = FormStartPosition.Manual;
            this.ShowIcon = false;

            ChangeRadius(20);
            
            this.ResumeLayout(false);

        }
    }
}
