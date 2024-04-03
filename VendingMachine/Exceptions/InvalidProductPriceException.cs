using System;

namespace VendingMachine.Exceptions
{
    public class InvalidProductPriceException : Exception
    {
        public InvalidProductPriceException() : base("The price can not be negative")
        {
        }
    }
}
