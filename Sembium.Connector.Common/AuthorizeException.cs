using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sembium.Connector
{
    public class AuthorizeException : UserException
    {
        public AuthorizeException(string message) : base(message)
        {
        }
    }
}
