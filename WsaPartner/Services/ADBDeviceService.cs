using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WsaPartner.Contracts.Services;

namespace WsaPartner.Services
{
    internal class ADBDeviceService : IADBDeviceService
    {
        public DeviceData CheckDevie(AdbClient adbClient)
        {
            IList<DeviceData> devices = adbClient.GetDevices();

            if (devices.Count <= 0) { return null; }

            foreach (DeviceData device in devices)
            {
                if (device.Model.Contains("Subsystem_for_Android_TM_"))
                {
                    return device;
                }
            }
            return null;
        }
    }
}
