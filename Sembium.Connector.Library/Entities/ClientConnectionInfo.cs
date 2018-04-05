using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Library.Entities
{
    public class ClientConnectionInfo
    {
        public string IPAddress { get; }
        public string DeviceId { get; }
        public long? OSSessionId { get; }
        public string OSVersion { get; }
        public string HardwareInfo { get; }

        public ClientConnectionInfo(string ipAddress, string deviceId, long? osSessionId, string osVersion, string hardwareInfo)
        {
            IPAddress = ipAddress;
            DeviceId = deviceId;
            OSSessionId = osSessionId;
            OSVersion = osVersion;
            HardwareInfo = hardwareInfo;
        }
    }
}
