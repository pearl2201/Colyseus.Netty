using System;
using System.Collections.Generic;
using System.Text;

namespace Coleseus.Shared.App.Impl
{
    public class InvalidCommandException : Exception
    {
        public InvalidCommandException(string message) : base(message)
        {

        }
    }
}
