using Sembium.Connector.Library.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Data.Connection
{
    public interface IClientConnectionInfoProvider
    {
        ClientConnectionInfo GetClientConnectionInfo();
    }
}
