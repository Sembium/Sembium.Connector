using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Data.Connection
{
    public class DataConnectionConfig : IDataConnectionConfig
    {
        public string DbName { get; }

        public string UserId { get; }
        public string UserFirstName { get; }
        public string UserMiddleName { get; }
        public string UserLastName { get; }       
        public bool UserIsPowerUser { get; }

        public DataConnectionConfig(string dbName, string userId, string userFirstName, string userMiddleName, string userLastName, bool userIsPowerUser)
        {
            DbName = dbName;
            UserId = userId;
            UserFirstName = userFirstName;
            UserMiddleName = userMiddleName;
            UserLastName = userLastName;
            UserIsPowerUser = userIsPowerUser;
        }
    }
}
