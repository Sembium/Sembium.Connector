using Sembium.Connector.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Data.Connection
{
    public interface IDataConnection
    {
        IEnumerable<T> GetItems<T>(Func<DbDataReader, T> readItemFunc, string sql, params SqlDataParameter[] parameters);
        T GetItem<T>(Func<DbDataReader, T> readItemFunc, string sql, params SqlDataParameter[] parameters);
        T GetValue<T>(string sql, params SqlDataParameter[] parameters);
        void ExecSql(string sql, params SqlDataParameter[] parameters);        
    }
}
