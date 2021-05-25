using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.App.Impl
{
    public class InvalidCommnadException : Exception
    {
        public InvalidCommnadException(string message) : base(message)
        {

        }
    }
}
