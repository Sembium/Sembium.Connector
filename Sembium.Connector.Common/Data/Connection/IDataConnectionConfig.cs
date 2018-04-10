using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Data.Connection
{
    public interface IDataConnectionConfig
    {
        string DbName { get; }
        string UserId { get; }
        string UserFirstName { get; }
        string UserMiddleName { get; }
        string UserLastName { get; }
        bool UserIsPowerUser { get; }
    }
}
