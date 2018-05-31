using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MousePad.Controllers
{
    public interface IDeviceStatus
    {
        bool IsEnabled { get; set; }
        void DeviceEnabled();
        void DeviceDisable();
        void ToggleEnable();
        Action OnEnableAction { get; set; }
        Action OnDisableAction { get; set; }
    }
}
