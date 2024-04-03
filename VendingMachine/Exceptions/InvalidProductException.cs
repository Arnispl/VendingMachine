using System;

namespace VendingMachine.Exceptions
{
    public class InvalidProductException : Exception
    {
        public InvalidProductException() : base("The product is not existing")
        {
        }
    }
}
