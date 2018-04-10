using Oracle.ManagedDataAccess.Client;
using Sembium.Connector.Data.Connection;
using Sembium.Connector.Data.Sql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Oracle
{
    public class OracleSqlDataConnection : ISqlDataConnection, IDisposable
    {
        private readonly ISqlConnectionStringProvider _connectionStringProvider;
        private readonly IDataConnectionConfigProvider _dataConnectionConfigProvider;

        private bool _inEnsureInitialized;
        private bool _initialized;
        private OracleConnection _dbConnection;

        public OracleSqlDataConnection(
            ISqlConnectionStringProvider connectionStringProvider,
            IDataConnectionConfigProvider dataConnectionConfigProvider)
        {
            _connectionStringProvider = connectionStringProvider;
            _dataConnectionConfigProvider = dataConnectionConfigProvider;
        }

        private void EnsureInitialized()
        {
            if (_inEnsureInitialized)
                return;

            _inEnsureInitialized = true;
            try
            {
                if (_initialized)
                    return;

                var config = _dataConnectionConfigProvider.GetConfig();

                var connectionString = _connectionStringProvider.GetConnectionString(config.DbName);

                Debug.Assert(!string.IsNullOrEmpty(connectionString));

                _dbConnection = new OracleConnection(connectionString);

                _dbConnection.Open();

                _initialized = true;
            }
            finally
            {
                _inEnsureInitialized = false;
            }
        }

        public IEnumerable<T> GetItems<T>(Func<DbDataReader, T> readItemFunc, string sql, params SqlDataParameter[] parameters)
        {
            EnsureInitialized();

            using (var command = new OracleCommand(sql, _dbConnection))
            {
                command.BindByName = true;

                command.Parameters.AddRange(GetOracleParameters(parameters));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            yield return readItemFunc(reader);
                        }
                    }
                }
            }
        }

        public void ExecSql(string sql, params SqlDataParameter[] parameters)
        {
            EnsureInitialized();

            using (var command = new OracleCommand(sql, _dbConnection))
            {
                command.BindByName = true;

                command.Parameters.AddRange(GetOracleParameters(parameters));

                command.ExecuteNonQuery();
            }
        }

        private Array GetOracleParameters(params SqlDataParameter[] parameters)
        {
            return parameters.Select(x => new OracleParameter(x.ParameterName, x.Value)).ToArray();
        }

        public void Dispose()
        {
            _dbConnection?.Dispose();
        }
    }
}
