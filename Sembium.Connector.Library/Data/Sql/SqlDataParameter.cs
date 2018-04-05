using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector.Data.Sql
{
    public class SqlDataParameter
    {
        public string ParameterName { get; }
        public object Value { get; }
        public SqlDataParameter(string parameterName, object value)
        {
            ParameterName = parameterName;
            Value = value;
        }
    }
}
