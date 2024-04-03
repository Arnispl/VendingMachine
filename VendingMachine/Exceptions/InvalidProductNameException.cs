using System;

namespace VendingMachine.Exceptions
{
    public class InvalidProductNameException : Exception
    {
        public InvalidProductNameException() : base("The product name is not valid")
        {
        }
    }
}
