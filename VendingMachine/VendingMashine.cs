using System;
using System.Linq;
using VendingMachine;
using VendingMachine.Exceptions;
public class MyVendingMachine : IVendingMachine
{
    public string Manufacturer { get; }
    public bool HasProducts => Products.Any(p => p.Available > 0);
    private Money _amount;
    public Money Amount
    {
        get { return _amount; }
    }
    public Product[] Products { get; private set; }

    public MyVendingMachine(string manufacturer, Product[] products)
    {
        Manufacturer = manufacturer;
        Products = products;
        _amount = new Money();
    }

    public Money InsertCoin(Money amount)
    {
        if (!IsValidMoney(amount))
        {
            throw new InvalidMoneyException();
        }

        int totalCents = _amount.Euros * 100 + _amount.Cents + amount.Euros * 100 + amount.Cents;
        _amount.Euros = totalCents / 100;
        _amount.Cents = totalCents % 100;

        return new Money();
    }

    public Money ReturnMoney()
    {
        Money returnedMoney = Amount;
        _amount = new Money();
        return returnedMoney;
    }

    public bool AddProduct(string name, Money price, int count)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidProductNameException();
        }

        if (price.Euros < 0 || price.Cents < 0)
        {
            throw new InvalidProductPriceException();
        }

        if (count < 0)
        {
            throw new InvalidProductCountException();
        }

        int existingProductIndex = -1;
        for (int i = 0; i < Products.Length; i++)
        {
            if (Products[i].Name == name)
            {
                existingProductIndex = i;
                break;
            }
        }

        if (existingProductIndex != -1)
        {
            Products[existingProductIndex].Available += count;
            return true;
        }

        Product newProduct = new Product
        {
            Name = name,
            Price = price,
            Available = count
        };

        Product[] updatedProducts = new Product[Products.Length + 1];
        for (int i = 0; i < Products.Length; i++)
        {
            updatedProducts[i] = Products[i];
        }
        updatedProducts[Products.Length] = newProduct;

        Products = updatedProducts;

        return true;
    }

    public bool UpdateProduct(int productNumber, string name, Money? price, int amount)
    {
        if (productNumber < 0 || productNumber >= Products.Length)
        {
            throw new InvalidProductException();
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidProductNameException();
        }

        if (price.HasValue && (price.Value.Euros < 0 || price.Value.Cents < 0))
        {
            throw new InvalidProductPriceException();
        }

        if (amount < 0)
        {
            throw new InvalidProductCountException();
        }

        if (productNumber < 0 || productNumber >= Products.Length)
        {
            return false;
        }

        Product productToUpdate = Products[productNumber];

        if (!string.IsNullOrWhiteSpace(name))
        {
            productToUpdate.Name = name;
        }

        if (price.HasValue && IsValidMoney(price.Value))
        {
            productToUpdate.Price = price.Value;
        }

        if (amount >= 0)
        {
            productToUpdate.Available = amount;
        }

        Products[productNumber] = productToUpdate;

        return true;
    }

    private bool IsValidMoney(Money money)
    {
        var validDenominations = new[]
        {
        new Money(1, 0),
        new Money(2, 0),
        new Money(0, 50),
        new Money(0, 20),
        new Money(0, 10)
        };

        return validDenominations.Any(d => d.Euros == money.Euros && d.Cents == money.Cents);
    }
}
