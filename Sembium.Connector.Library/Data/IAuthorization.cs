using Sembium.Connector.Data.Connection;
using Sembium.Connector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Data
{
    public interface IAuthorization
    {
        void CheckUserActivity(int activityCode);
    }
}
