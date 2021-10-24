using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpAdbClient;

namespace WsaPartner.Contracts.Services
{
    public interface IADBDeviceService
    {
        public DeviceData CheckDevie(AdbClient adbClient);
    }
}
