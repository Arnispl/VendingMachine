using System;

namespace VendingMachine
{

    class Program
    {
        static void Main(string[] args)
        {
            Product[] initialProducts = new Product[]
            {
            new Product { Name = "NotCola", Price = new Money { Euros = 1, Cents = 50 }, Available = 10 },
            new Product { Name = "Chips", Price = new Money { Euros = 1, Cents = 20 }, Available = 8 },
            new Product { Name = "Water still", Price = new Money { Euros = 1, Cents = 40 }, Available = 12 },
            new Product { Name = "Water sparkling", Price = new Money { Euros = 1, Cents = 40 }, Available = 6 },
            new Product { Name = "Energy Bar", Price = new Money { Euros = 0, Cents = 80 }, Available = 8 },
            new Product { Name = "Carrots", Price = new Money { Euros = 0, Cents = 90 }, Available = 4 },
            new Product { Name = "Nuts&berries", Price = new Money { Euros = 1, Cents = 20 }, Available = 10 },
            new Product { Name = "Last dinner", Price = new Money { Euros = 4, Cents = 0 }, Available = 2 },
            new Product { Name = "Beef jerky", Price = new Money { Euros = 1, Cents = 90 }, Available = 7 },
            new Product { Name = "Smelly Belly", Price = new Money { Euros = 2, Cents = 50 }, Available = 8 }

            };

            IVendingMachine vendingMachine = new MyVendingMachine("Give me all your money Co.", initialProducts);

            Console.WriteLine($"Hello from {vendingMachine.Manufacturer}'s Vending Machine!");

            while (true)
            {
                Console.WriteLine("\nMenu:");
                Console.WriteLine("1. Insert Coins");
                Console.WriteLine("2. Display Products");
                Console.WriteLine("3. Select a Product");
                Console.WriteLine("4. Return Money");
                Console.WriteLine("5. Exit");
                Console.Write("Select an option: ");

                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter the amount to insert (Euros): ");
                        int euros = int.Parse(Console.ReadLine());
                        Console.Write("Enter the amount to insert (Cents): ");
                        int cents = int.Parse(Console.ReadLine());

                        Money insertedMoney = new Money { Euros = euros, Cents = cents };
                        Money remainingMoney = vendingMachine.InsertCoin(insertedMoney);
                        Console.WriteLine($"Inserted {insertedMoney.Euros} Euros and {insertedMoney.Cents} Cents.");
                        Console.WriteLine($"Current Balance: {vendingMachine.Amount.Euros} Euros and {vendingMachine.Amount.Cents} Cents.");
                        break;

                    case 2:
                        Console.WriteLine("Available Products:");
                        for (int i = 0; i < vendingMachine.Products.Length; i++)
                        {
                            var product = vendingMachine.Products[i];
                            Console.WriteLine($"{i + 1}. {product.Name}: {product.Price.Euros} Euros and {product.Price.Cents} Cents ({product.Available} available)");
                        }
                        break;

                    case 3:
                        Console.Write("Enter the product number: ");
                        int selectedProductNumber = int.Parse(Console.ReadLine());

                        if (selectedProductNumber >= 1 && selectedProductNumber <= vendingMachine.Products.Length)
                        {
                            var selectedProduct = vendingMachine.Products[selectedProductNumber - 1];
                            string productName = selectedProduct.Name;
                            Money productPrice = selectedProduct.Price;
                            int totalPriceInCents = productPrice.Euros * 100 + productPrice.Cents;

                            if (selectedProduct.Available > 0 && totalPriceInCents <= vendingMachine.Amount.Euros * 100 + vendingMachine.Amount.Cents)
                            {
                                Console.WriteLine($"Selected {productName} for {productPrice.Euros} Euros and {productPrice.Cents} Cents.");
                                Console.WriteLine($"Dispensing {productName}...");

                                selectedProduct.Available--;

                                int changeInCents = vendingMachine.Amount.Euros * 100 + vendingMachine.Amount.Cents - totalPriceInCents;
                                int changeEuros = changeInCents / 100;
                                int changeCents = changeInCents % 100;

                                Money change = new Money
                                {
                                    Euros = changeEuros,
                                    Cents = changeCents
                                };

                                vendingMachine.ReturnMoney();
                                vendingMachine.InsertCoin(change);

                                Console.WriteLine($"Enjoy your {productName}!");
                                Console.WriteLine($"Remaining Balance: {vendingMachine.Amount.Euros} Euros and {vendingMachine.Amount.Cents} Cents.");
                            }
                            else
                            {
                                Console.WriteLine("Product not available or insufficient balance.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid product number.");
                        }
                        break;


                    case 4:
                        Money returnedMoney = vendingMachine.ReturnMoney();
                        Console.WriteLine($"Returned {returnedMoney.Euros} Euros and {returnedMoney.Cents} Cents.");
                        break;

                    case 5:
                        Environment.Exit(0);
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please select a valid option.");
                        break;
                }
            }
        }
    }
}