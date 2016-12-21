using System;

namespace MineFlags.Exceptions
{
    class InvalidDIException : Exception
    {
        public InvalidDIException(string message) : base(message) { }
    }
}
