namespace VendingMachine
{
    public interface IVendingMachine
    {
        string Manufacturer { get; }

        bool HasProducts { get; }

        Money Amount { get; }

        Product[] Products { get; }

        Money InsertCoin(Money amount);

        Money ReturnMoney();

        bool AddProduct(string name, Money price, int count);

        bool UpdateProduct(int productNumber, string name, Money? price, int amount);      
    }
}
