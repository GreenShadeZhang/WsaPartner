using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WsaPartner.Contracts.Services;
using WsaPartner.Helpers;

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

        public string VersionComparison(PackageManager packageManager, string packageName, string targetVersionCode)
        {
            var vInfo = packageManager.GetVersionInfo(packageName);

            if (vInfo == null)
            {
                return "InstallApp".GetLocalized();
            }
            else if (vInfo.VersionCode < int.Parse(targetVersionCode))
            {
                return "UpdateApp".GetLocalized();
            }
            else
            {
                return "ReInstallApp".GetLocalized();
            }
        }
    }
}
