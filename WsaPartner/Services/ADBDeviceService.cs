using SharpAdbClient;
using SharpAdbClient.DeviceCommands;
using System.Collections.Generic;
using WsaPartner.Contracts.Services;
using WsaPartner.Helpers;

namespace WsaPartner.Services
{
    internal class ADBDeviceService : IADBDeviceService
    {
        public DeviceData CheckDevie(AdbClient adbClient)
        {
            DeviceData deviceData = null;

            IList<DeviceData> devices = adbClient.GetDevices();

            if (devices != null && devices.Count > 0)
            {
                foreach (DeviceData device in devices)
                {
                    if (device.Model.Contains("Subsystem_for_Android_TM_"))
                    {
                        deviceData = device;
                    }
                }
            }

            return deviceData;
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
