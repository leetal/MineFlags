using System;

namespace MineFlags.Exceptions
{
    class StateException : Exception
    {
        public StateException(string messsage, Exception e) : base(messsage, e) { }
    }
}
