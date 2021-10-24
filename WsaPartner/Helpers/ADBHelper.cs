using SharpAdbClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WsaPartner.Helpers
{
    internal static class ADBHelper
    {
        public static DeviceMonitor Monitor = new DeviceMonitor(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)));
        static ADBHelper()
        {
            Monitor.Start();
        }
    }
}
