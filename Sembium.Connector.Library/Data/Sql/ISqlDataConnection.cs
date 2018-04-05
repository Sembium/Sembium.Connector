using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Data.Sql
{
    public interface ISqlDataConnection
    {
        IEnumerable<T> GetItems<T>(Func<DbDataReader, T> readItemFunc, string sql, params SqlDataParameter[] parameters);
        void ExecSql(string sql, params SqlDataParameter[] parameters);
    }
}
