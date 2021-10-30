using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAdbClient;
using SharpAdbClient.DeviceCommands;

namespace WsaPartner.Contracts.Services
{
    public interface IADBDeviceService
    {
        public DeviceData CheckDevie(AdbClient adbClient);

        public string VersionComparison(PackageManager packageManager, string packageName, string targetVersionCode);
    }
}
