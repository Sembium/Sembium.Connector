using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Data.Sql
{
    public interface ISqlConnectionStringProvider
    {
        string GetConnectionString(string dbName);
    }
}
