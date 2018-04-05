using System;
using Sembium.Connector.Library.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Sembium.Connector.Data.Connection
{
    public class ClientConnectionInfoProvider : IClientConnectionInfoProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        public ClientConnectionInfoProvider(
            IServiceProvider serviceProvider,
            IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public ClientConnectionInfo GetClientConnectionInfo()
        {
            var httpContextAccessor = _serviceProvider.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            var ipAddress = GetClientIPAddress(httpContext);

            var deviceId = httpContext.Request.Headers["ClientDeviceId"];
            var osSessionId = (long.TryParse(httpContext.Request.Headers["ClientOSSessionId"], out long x) ? (long?)x : null);
            var osVersion = httpContext.Request.Headers["ClientOSVersion"];
            var hardwareInfo = httpContext.Request.Headers["ClientHardwareInfo"];

            return new ClientConnectionInfo(ipAddress, deviceId, osSessionId, osVersion, hardwareInfo);
        }

        private string GetClientIPAddress(HttpContext httpContext)
        {
            var clientIPAddressHeaderName = _configuration["ClientIPAddressHeaderName"];

            if (!string.IsNullOrEmpty(clientIPAddressHeaderName))
            {
                var headerValue = httpContext.Request.Headers[clientIPAddressHeaderName].ToString().Trim();

                if (string.IsNullOrEmpty(headerValue))
                {
                    throw new Exception($"Header '{clientIPAddressHeaderName}' not found");
                }

                return headerValue.Split(',', ';').LastOrDefault().Trim();
            }

            return httpContext.Connection?.RemoteIpAddress?.MapToIPv4()?.ToString();
        }
    }
}
