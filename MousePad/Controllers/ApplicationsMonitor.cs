using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MousePad.Controllers
{
    public class ApplicationsMonitor
    {
        private ApplicationMonitorFile AppFileConfig;
        private List<IDeviceStatus> devices;
        public ApplicationsMonitor()
        {
            devices = new List<IDeviceStatus>(3);
            LoadConfig();
            Task.Factory.StartNew(() =>
            {
                // CheckApplications();
                Thread.Sleep(2500);
            });
        }
        public void AddDevice(IDeviceStatus device)
        {
            if (devices.Find((x) => x == device) != null)
                return;
            devices.Add(device);
        }
        public void RemoveDevice(IDeviceStatus device)
        {
            devices.Remove(device);
        }
        public void LoadConfig()
        {
            var content = File.ReadAllText("apps_monitor.json");
            AppFileConfig = Newtonsoft.Json.JsonConvert.DeserializeObject<ApplicationMonitorFile>(content);
        }
        private void CheckApplications()
        {
            bool disable = false;
            foreach(var app in AppFileConfig.Apps)
            {
                var processes = Process.GetProcessesByName(app.ProgramName);
                if (processes != null && processes.Length > 0)
                    disable = true;
            }
            for(int i=0;i<devices.Count && disable; i++)
            {
                if(devices[i].IsEnabled)
                    devices[i].DeviceDisable();
            }
        }
    }
    class ApplicationMonitorFile
    {
        public List<ApplicationParameters> Apps { get; set; }
    }
    class ApplicationParameters
    {
        public string ProgramName { get; set; }
        public List<string> Parameters { get; set; }
    }
}
