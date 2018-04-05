using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Data.Connection
{
    public interface IIpAddressUtils
    {
        bool IpAddressMatchesPattern(string ipString, string ipPattern);
    }
}
