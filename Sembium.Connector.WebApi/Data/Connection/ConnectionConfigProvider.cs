using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sembium.Connector.Data.Connection
{
    public class ConnectionConfigProvider : IDataConnectionConfigProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public ConnectionConfigProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDataConnectionConfig GetConfig()
        {
            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var dbName = httpContext.Request.Headers["DBName"].FirstOrDefault()?.ToLowerInvariant();
            var userId = httpContext.User.Claims.FirstOrDefault(x => x.Type.Equals("sub")).Value;

            var userFirstName = httpContext.User.Claims.FirstOrDefault(x => x.Type.Equals("given_name")).Value;
            var userMiddleName = httpContext.User.Claims.FirstOrDefault(x => x.Type.Equals("middle_name")).Value;
            var userLastName = httpContext.User.Claims.FirstOrDefault(x => x.Type.Equals("family_name")).Value;

            var userIsPowerUser = httpContext.User.Claims.Any(x => x.Type.Equals("role") && string.Equals(x.Value, "admin", StringComparison.InvariantCultureIgnoreCase));

            return new DataConnectionConfig(dbName, userId, userFirstName, userMiddleName, userLastName, userIsPowerUser);
        }
    }
}
