using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Entities
{
    public class UserInfo
    {
        public int UserCode { get;  }
        public string FirstName { get; }
        public string MiddleName { get; }
        public string LastName { get; }
        public bool IsPowerUser { get; }

        public UserInfo(int userCode, string firstName, string middleName, string lastName, bool isPowerUser)
        {
            UserCode = userCode;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            IsPowerUser = isPowerUser;
        }
    }
}
