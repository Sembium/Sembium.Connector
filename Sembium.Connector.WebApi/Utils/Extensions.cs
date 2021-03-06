﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Sembium.Connector.WebApi.Utils
{
    public static class Extensions
    {
        public static string GetInnerMessages(this Exception exception)
        {
            var e = exception;
            var result = e.Message;

            while (e.InnerException != null)
            {
                result = result + Environment.NewLine + e.InnerException.Message;
                e = e.InnerException;
            }

            return result;
        }

        public static string GetAggregateMessages(this Exception exception)
        {
            if (!(exception is AggregateException))
            {
                return exception.GetInnerMessages();
            }

            var aggregateMessages = (exception as AggregateException).InnerExceptions.Select(x => x.GetAggregateMessages());

            return string.Join(Environment.NewLine, aggregateMessages);
        }
    }
}
