using System;

namespace VendingMachine.Exceptions
{
    public class InvalidProductCountException : Exception
    {
        public InvalidProductCountException() : base("The product count can not be negative")
        {
        }
    }
}
