using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BitDiamond.Web.Infrastructure.Exceptions
{
    public class MalformedApiArgumentsException: Exception
    {
        public MalformedApiArgumentsException() { }

        public MalformedApiArgumentsException(string message)
        : base(message)
        { }
    }
}