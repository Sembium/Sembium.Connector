using Sembium.Connector.Data.Sql;
using Sembium.Connector.Entities;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Sembium.Connector.Data.Connection
{
    public class DataConnection : IDataConnection
    {
        private readonly ISqlDataConnection _sqlDataConnection;
        private readonly IDataConnectionConfigProvider _dataConnectionConfigProvider;
        private readonly IClientConnectionInfoProvider _clientConnectionInfoProvider;
        private readonly IIpAddressUtils _ipAddressUtils;

        private bool _inEnsureInitialized;
        private bool _initialized;

        public DataConnection(
            ISqlDataConnection sqlDataConnection,
            IDataConnectionConfigProvider dataConnectionConfigProvider,
            IClientConnectionInfoProvider clientConnectionInfoProvider,
            IIpAddressUtils ipAddressUtils)
        {
            _sqlDataConnection = sqlDataConnection;
            _dataConnectionConfigProvider = dataConnectionConfigProvider;
            _clientConnectionInfoProvider = clientConnectionInfoProvider;
            _ipAddressUtils = ipAddressUtils;
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

                var userInfo = GetUserInfo(config.UserId);

                if (userInfo == null)
                {
                    throw new UserException("User not found");
                }

                CheckUserConnection(userInfo.UserCode);
                CheckUserNames(userInfo, config);

                SetDatabaseLoginContext(userInfo.UserCode, GetDbTimeZoneDateTime(), false, null);

                if (!config.UserIsPowerUser)
                {
                    SetUserLastLoginInfo(userInfo.UserCode);
                }

                _initialized = true;
            }
            finally
            {
                _inEnsureInitialized = false;
            }
        }

        public T GetValue<T>(string sql, params SqlDataParameter[] parameters)
        {
            return GetItem(reader => reader.IsDBNull(0) ? default(T) : reader.GetFieldValue<T>(0), sql, parameters);
        }

        public T GetItem<T>(Func<DbDataReader, T> readItemFunc, string sql, params SqlDataParameter[] parameters)
        {
            return GetItems(readItemFunc, sql, parameters).FirstOrDefault();
        }

        public IEnumerable<T> GetItems<T>(Func<DbDataReader, T> readItemFunc, string sql, params SqlDataParameter[] parameters)
        {
            EnsureInitialized();
            return _sqlDataConnection.GetItems(readItemFunc, sql, parameters);
        }

        public void ExecSql(string sql, params SqlDataParameter[] parameters)
        {
            EnsureInitialized();
            _sqlDataConnection.ExecSql(sql, parameters);
        }

        private void SetDatabaseLoginContext(long userCode, DateTime currentDateTime, bool isReadOnly, string dbId)
        {
            ExecSql(
                "begin" + Environment.NewLine +
                "  LoginContext.SetContext(:USER_CODE, 0, :CONTEXT_DATE_TIME, :IS_CONTEXT_READ_ONLY, :DB_ID);" + Environment.NewLine +
                "end;",
                new SqlDataParameter("USER_CODE", userCode),
                new SqlDataParameter("CONTEXT_DATE_TIME", currentDateTime),
                new SqlDataParameter("IS_CONTEXT_READ_ONLY", (isReadOnly ? 1 : 0)),
                new SqlDataParameter("DB_ID", dbId));
        }

        private UserInfo ReadUserInfo(DbDataReader reader)
        {
            return
                new UserInfo(
                    userCode: reader.GetInt32(0),
                    firstName: reader.GetString(1),
                    middleName: (reader.IsDBNull(2) ? null : reader.GetString(2)),
                    lastName: reader.GetString(3),
                    isPowerUser: false
                );
        }

        private UserInfo GetUserInfo(string userId)
        {
            return
                GetItem(ReadUserInfo,
                    "select"                                                + Environment.NewLine +
                    "  u.EMPLOYEE_CODE,"                                    + Environment.NewLine +
                    "  e.FIRST_NAME,"                                       + Environment.NewLine +
                    "  e.MIDDLE_NAME,"                                      + Environment.NewLine +
                    "  e.LAST_NAME"                                         + Environment.NewLine +
                    ""                                                      + Environment.NewLine +
                    "from"                                                  + Environment.NewLine +
                    "  USERS u,"                                            + Environment.NewLine +
                    "  EMPLOYEES e"                                         + Environment.NewLine +
                    ""                                                      + Environment.NewLine +
                    "where"                                                 + Environment.NewLine +
                    "  (u.EMPLOYEE_CODE = e.EMPLOYEE_CODE) and"             + Environment.NewLine +
                    "  (u.EXTERNAL_IDENTIFIER = :USER_ID)",
                    new SqlDataParameter("USER_ID", userId));
        }

        private DateTime GetDbTimeZoneDateTime()
        {
            var timeZoneId = GetDatabaseTimeZoneId();

            var result = DateTime.Now;
            if (!string.IsNullOrEmpty(timeZoneId))
            {
                result = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(result, timeZoneId);
            }

            return result;
        }

        private string GetDatabaseTimeZoneId()
        {
            return GetValue<string>("select TIME_ZONE_ID from COMMON_OPTIONS where (CODE = 1)");
        }

        private void SetUserLastLoginInfo(long userCode)
        {
            var clientConnectionInfo = _clientConnectionInfoProvider.GetClientConnectionInfo();

            ExecSql(
                "update" + Environment.NewLine +
                "  USERS" + Environment.NewLine +
                "set" + Environment.NewLine +
                "  LAST_LOGIN_DATE = ContextDate," + Environment.NewLine +
                "  LAST_LOGIN_TIME = ContextTime," + Environment.NewLine +
                "  LAST_LOGIN_COMPUTER_NAME = :LAST_LOGIN_COMPUTER_NAME," + Environment.NewLine +
                "  LAST_LOGIN_WINDOWS_SESSION_ID = :LAST_LOGIN_WINDOWS_SESSION_ID," + Environment.NewLine +
                "  LAST_LOGIN_WINDOWS_VERSION = :LAST_LOGIN_WINDOWS_VERSION," + Environment.NewLine +
                "  LAST_LOGIN_HARDWARE_INFO = :LAST_LOGIN_HARDWARE_INFO" + Environment.NewLine +
                "where" + Environment.NewLine +
                "  (EMPLOYEE_CODE = :EMPLOYEE_CODE)",

                new SqlDataParameter("LAST_LOGIN_COMPUTER_NAME", clientConnectionInfo.DeviceId),
                new SqlDataParameter("LAST_LOGIN_WINDOWS_SESSION_ID", clientConnectionInfo.OSSessionId),
                new SqlDataParameter("LAST_LOGIN_WINDOWS_VERSION", clientConnectionInfo.OSVersion),
                new SqlDataParameter("LAST_LOGIN_HARDWARE_INFO", clientConnectionInfo.HardwareInfo),
                new SqlDataParameter("EMPLOYEE_CODE", userCode));
        }

        private (string IPAddressPattern, int AccessTypeCode) ReadAccessRule(DbDataReader reader)
        {
            return (IPAddressPattern: reader.GetString(0), AccessTypeCode: reader.GetInt32(1));
        }

        private bool GetAccessRulesExist()
        {
            return (GetValue<long>("select Cast(Count(*) as NUMBER(10)) from ACCESS_RULES where (IS_ACTIVE = 1)") > 0);
        }

        private IEnumerable<(string IPAddressPattern, int AccessTypeCode)> GetUserAccessRules(long userCode)
        {
            return
                GetItems(
                    ReadAccessRule,
                    "select" + Environment.NewLine +
                    "  ar.IP_ADDRESS_PATTERN," + Environment.NewLine +
                    "  ar.ACCESS_TYPE_CODE" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "from" + Environment.NewLine +
                    "  OCCUPATION_EMPLOYEES oe," + Environment.NewLine +
                    "  OCCUPATION_WORK_DEPTS owd," + Environment.NewLine +
                    "  DEPT_RELATIONS dr," + Environment.NewLine +
                    "  ACCESS_RULES ar" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "where" + Environment.NewLine +
                    "  (oe.EMPLOYEE_CODE = :EMPLOYEE_CODE) and" + Environment.NewLine +
                    "  (Trunc(SysDate) between oe.ASSIGNMENT_BEGIN_DATE and oe.ASSIGNMENT_END_DATE) and" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "  (oe.OCCUPATION_CODE = owd.OCCUPATION_CODE) and" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "  (dr.DESCENDANT_DEPT_CODE = owd.DEPT_CODE) and" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "  ( (ar.INCLUDE_DEPT_DESCENDANTS = 1) or" + Environment.NewLine +
                    "    (dr.DESCENDANT_DEPT_CODE = dr.ANCESTOR_DEPT_CODE)" + Environment.NewLine +
                    "  ) and" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "  (ar.DEPT_CODE = dr.ANCESTOR_DEPT_CODE) and" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "  (ar.IS_ACTIVE = 1)" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "union" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "select" + Environment.NewLine +
                    "  ar.IP_ADDRESS_PATTERN," + Environment.NewLine +
                    "  ar.ACCESS_TYPE_CODE" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "from" + Environment.NewLine +
                    "  ACCESS_RULES ar" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "where" + Environment.NewLine +
                    "  (ar.EMPLOYEE_CODE = :EMPLOYEE_CODE) and" + Environment.NewLine +
                    "  (ar.IS_ACTIVE = 1)" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "union" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "select" + Environment.NewLine +
                    "  ar.IP_ADDRESS_PATTERN," + Environment.NewLine +
                    "  ar.ACCESS_TYPE_CODE" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "from" + Environment.NewLine +
                    "  ACCESS_RULES ar" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "where" + Environment.NewLine +
                    "  (ar.DEPT_CODE is null) and" + Environment.NewLine +
                    "  (ar.EMPLOYEE_CODE is null) and" + Environment.NewLine +
                    "  (ar.IS_ACTIVE = 1)" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "order by" + Environment.NewLine +
                    "  IP_ADDRESS_PATTERN," + Environment.NewLine +
                    "  ACCESS_TYPE_CODE",
                    new SqlDataParameter("EMPLOYEE_CODE", userCode)
                );
        }

        private void CheckUserConnection(long userCode)
        {
            if (!GetAccessRulesExist())
            {
                return;
            }

            var clientConnectionInfo = _clientConnectionInfoProvider.GetClientConnectionInfo();

            if ((clientConnectionInfo.IPAddress == "0.0.0.1") || (clientConnectionInfo.IPAddress == "127.0.0.1"))
            {
                return;
            }

            var userAccessRules = GetUserAccessRules(userCode);

            var matchigAccessRules =
                    userAccessRules
                    .Where(x => string.IsNullOrEmpty(x.IPAddressPattern) || _ipAddressUtils.IpAddressMatchesPattern(clientConnectionInfo.IPAddress, x.IPAddressPattern))
                    .ToList();

            var IPAddressAllowed = matchigAccessRules.Any(x => x.AccessTypeCode == 1);
            var IPAddressDenied = matchigAccessRules.Any(x => x.AccessTypeCode == 2);

            if ((!IPAddressAllowed) || IPAddressDenied)
            {
                throw new AuthorizeException($"IP address access is denied: {clientConnectionInfo.IPAddress}");
            }
        }

        private void CheckUserNames(UserInfo userInfo, IDataConnectionConfig config)
        {
            if (((!string.IsNullOrEmpty(config.UserFirstName)) && (!config.UserFirstName.Equals(userInfo.FirstName))) ||
                ((!string.IsNullOrEmpty(config.UserMiddleName)) && (!config.UserMiddleName.Equals(userInfo.MiddleName))) ||
                ((!string.IsNullOrEmpty(config.UserLastName)) && (!config.UserLastName.Equals(userInfo.LastName))))
            {
                throw new AuthorizeException("User and employee names differ");
            }
        }
    }
}
