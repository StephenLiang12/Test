using System;

namespace Market.Exceptions
{
    public class NoSymolFoundException : ArgumentException
    {
        public NoSymolFoundException(string message) : base(message)
        {
        }
    }
}