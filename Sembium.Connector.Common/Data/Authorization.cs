using Sembium.Connector.Data.Connection;
using Sembium.Connector.Data.Sql;
using Sembium.Connector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Common;

namespace Sembium.Connector.Data
{
    public class Authorization : IAuthorization
    {
        private readonly IDataConnection _dataConnection;

        public Authorization(IDataConnection dataConnection)
        {
            _dataConnection = dataConnection;
        }

        public void CheckUserActivity(int activityCode)
        {
            var resultActivityCode = 
                _dataConnection.GetValue<long>(
                    "select" + Environment.NewLine +
                    "  ua.ACTIVITY_CODE" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "from" + Environment.NewLine +
                    "  USER_ACTIVITIES ua" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "where" + Environment.NewLine +
                    "  (ua.ACTIVITY_CODE = :ACTIVITY_CODE) and" + Environment.NewLine +
                    "  (ua.EMPLOYEE_CODE = LoginContext.UserCode)" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "union" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "select" + Environment.NewLine +
                    "  uga.ACTIVITY_CODE" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "from" + Environment.NewLine +
                    "  USER_GROUP_USERS ugu," + Environment.NewLine +
                    "  USER_GROUP_ACTIVITIES uga" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "where" + Environment.NewLine +
                    "  (ugu.USER_GROUP_CODE = uga.USER_GROUP_CODE) and" + Environment.NewLine +
                    "  (uga.ACTIVITY_CODE = :ACTIVITY_CODE) and" + Environment.NewLine +
                    "  (ugu.EMPLOYEE_CODE = LoginContext.UserCode)" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "union" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "select" + Environment.NewLine +
                    "  sra.ACTIVITY_CODE" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "from" + Environment.NewLine +
                    "  OCCUPATION_EMPLOYEES oe," + Environment.NewLine +
                    "  OCCUPATION_SYS_ROLES osr," + Environment.NewLine +
                    "  SYS_ROLE_ACTIVITIES sra" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "where" + Environment.NewLine +
                    "  (sra.ACTIVITY_CODE = :ACTIVITY_CODE) and" + Environment.NewLine +
                    "  (oe.EMPLOYEE_CODE = LoginContext.UserCode) and" + Environment.NewLine +
                    "  (ContextDate between oe.ASSIGNMENT_BEGIN_DATE and oe.ASSIGNMENT_END_DATE) and" + Environment.NewLine +
                    "  (oe.OCCUPATION_CODE = osr.OCCUPATION_CODE) and" + Environment.NewLine +
                    "  (osr.SYS_ROLE_CODE = sra.SYS_ROLE_CODE)" + Environment.NewLine +
                    "" + Environment.NewLine +
                    "order by" + Environment.NewLine +
                    "  1",
                    new SqlDataParameter("ACTIVITY_CODE", activityCode));

            if (resultActivityCode == 0)
            {
                throw new AuthorizeException("User has no rights for this operation");
            }
        }
    }
}
