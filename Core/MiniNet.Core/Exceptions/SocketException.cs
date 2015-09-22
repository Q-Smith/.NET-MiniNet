using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniNet.Exceptions
{
    public class SocketException : Exception
    {
        public int StatusCode { get; private set; }

        public SocketException(int statusCode)
            : base()
        {
            StatusCode = statusCode;
        }

        public SocketException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public SocketException(int statusCode, string message, Exception innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
