using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sembium.Connector.WebApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Sembium.Connector.WebApi
{
    public class ErrorHandlingMiddleware<TUserException, TAuthenticationException> where TUserException : Exception where TAuthenticationException : TUserException
    {
        private readonly RequestDelegate _next;

        public ILoggerFactory _loggerFactory { get; }

        public ErrorHandlingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _loggerFactory = loggerFactory;
        }

        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (TUserException ex)
            {
                _loggerFactory.CreateLogger("Errors").LogError(0, ex, ex.GetAggregateMessages());
                await HandleUserExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                _loggerFactory.CreateLogger("Errors").LogError(0, ex, ex.GetAggregateMessages());
                throw;
            }
        }

        private Task HandleUserExceptionAsync(HttpContext context, TUserException exception)
        {
            var result = JsonConvert.SerializeObject(new { error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)GetUserExceptionHttpStatusCode(exception);
            return context.Response.WriteAsync(result);
        }

        private static HttpStatusCode GetUserExceptionHttpStatusCode(TUserException exception)
        {
            if ((typeof(TAuthenticationException) != typeof(TUserException)) &&
                 (exception is TAuthenticationException))
            {
                return HttpStatusCode.Unauthorized;
            }

            return HttpStatusCode.InternalServerError;
        }
    }
}
