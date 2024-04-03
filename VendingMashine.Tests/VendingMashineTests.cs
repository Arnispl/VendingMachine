using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using VendingMachine;
using VendingMachine.Exceptions;
using FluentAssertions;
using System;


namespace VendingMashine.Tests
{
    [TestClass]
    public class VendingMashineTests
    {
        private MyVendingMachine _myVendingMachine;

        [TestInitialize]
        public void Setup()
        {
            Product[] initialProducts = new Product[]
            {
                new Product { Name = "Product1", Price = new Money(1, 0), Available = 10 },
                new Product { Name = "Product2", Price = new Money(2, 50), Available = 5 }
            };

            _myVendingMachine = new MyVendingMachine("ManufacturerName", initialProducts);
        }

        [TestMethod]
        public void ManufacturerProperty_ShouldReturnCorrectManufacturer()
        {
            var expectedManufacturer = "Test Manufacturer";
            var vendingMachine = new MyVendingMachine(expectedManufacturer, new Product[0]);

            var actualManufacturer = vendingMachine.Manufacturer;

            actualManufacturer.Should().Be(expectedManufacturer);
        }

        [TestMethod]
        public void HasProducts_WhenNoProducts_ShouldReturnFalse()
        {
            var vendingMachine = new MyVendingMachine("Test Manufacturer", new Product[0]);

            vendingMachine.HasProducts.Should().BeFalse();
        }

        [TestMethod]
        public void HasProducts_WhenProductsNotAvailable_ShouldReturnFalse()
        {
            var products = new Product[] { new Product { Name = "Test Product", Price = new Money(1, 0), Available = 0 } };
            var vendingMachine = new MyVendingMachine("Test Manufacturer", products);

            vendingMachine.HasProducts.Should().BeFalse();
        }

        [TestMethod]
        public void HasProducts_WhenProductsAvailable_ShouldReturnTrue()
        {
            var products = new Product[] { new Product { Name = "Test Product", Price = new Money(1, 0), Available = 1 } };
            var vendingMachine = new MyVendingMachine("Test Manufacturer", products);

            vendingMachine.HasProducts.Should().BeTrue();
        }

        [TestMethod]
        public void InsertCoin_ValidMoney_ShouldIncreaseAmount()
        {
            Money initialAmount = _myVendingMachine.Amount;
            Money coinToInsert = new Money(1, 0); 

            Money returnedChange = _myVendingMachine.InsertCoin(coinToInsert);

            Money expectedAmount = new Money(
                initialAmount.Euros + coinToInsert.Euros,
                initialAmount.Cents + coinToInsert.Cents
            );

            _myVendingMachine.Amount.Euros.Should().Be(expectedAmount.Euros);
            _myVendingMachine.Amount.Cents.Should().Be(expectedAmount.Cents);
            returnedChange.Euros.Should().Be(0);
            returnedChange.Cents.Should().Be(0);
        }

        [TestMethod]
        public void InsertMultipleValidCoins_CumulativeAdditionShouldBeCorrect()
        {
            Money firstCoin = new Money(1, 0); 
            Money secondCoin = new Money(0, 50); 
            Money thirdCoin = new Money(2, 0); 

            _myVendingMachine.InsertCoin(firstCoin);
            _myVendingMachine.InsertCoin(secondCoin);
            _myVendingMachine.InsertCoin(thirdCoin);

            int expectedEuros = firstCoin.Euros + secondCoin.Euros + thirdCoin.Euros; 
            int expectedCents = firstCoin.Cents + secondCoin.Cents + thirdCoin.Cents;

            _myVendingMachine.Amount.Euros.Should().Be(expectedEuros);
            _myVendingMachine.Amount.Cents.Should().Be(expectedCents);
        }

        [TestMethod]
        public void InsertCoin_InvalidMoney_ShouldThrowInvalidMoneyException()
        {
            Money invalidMoney = new Money(5, 0);

            Action act = () => _myVendingMachine.InsertCoin(invalidMoney);

            act.Should().Throw<InvalidMoneyException>().WithMessage("Invalid money inserted.");
        }

        [TestMethod]
        public void ReturnMoney_ShouldReturnCorrectAmountAndResetBalance()
        {
            _myVendingMachine.InsertCoin(new Money(1, 0));

            Money returnedMoney = _myVendingMachine.ReturnMoney();

            Money expectedReturnedMoney = new Money(1, 0); 
            Money expectedBalanceAfterReturn = new Money();

            returnedMoney.Euros.Should().Be(expectedReturnedMoney.Euros);
            returnedMoney.Cents.Should().Be(expectedReturnedMoney.Cents);
            _myVendingMachine.Amount.Euros.Should().Be(expectedBalanceAfterReturn.Euros);
            _myVendingMachine.Amount.Cents.Should().Be(expectedBalanceAfterReturn.Cents);

        }

        [TestMethod]
        public void ReturnMoney_AfterMultipleCoinsInserted_ShouldReturnCorrectTotalAmount()
        {
            Money firstCoin = new Money(1, 0); 
            Money secondCoin = new Money(0, 50); 
            Money thirdCoin = new Money(2, 0); 

            _myVendingMachine.InsertCoin(firstCoin);
            _myVendingMachine.InsertCoin(secondCoin);
            _myVendingMachine.InsertCoin(thirdCoin);

            Money returnedMoney = _myVendingMachine.ReturnMoney();

            int expectedTotalEuros = firstCoin.Euros + secondCoin.Euros + thirdCoin.Euros; 
            int expectedTotalCents = firstCoin.Cents + secondCoin.Cents + thirdCoin.Cents; 

            expectedTotalEuros += expectedTotalCents / 100;
            expectedTotalCents %= 100;

            returnedMoney.Euros.Should().Be(expectedTotalEuros);
            returnedMoney.Cents.Should().Be(expectedTotalCents);
        }

        [TestMethod]
        public void ReturnMoney_WhenNoMoneyInserted_ShouldReturnZero()
        {
            Money returnedMoney = _myVendingMachine.ReturnMoney();

            Money expectedReturnedMoney = new Money(0, 0);
            returnedMoney.Euros.Should().Be(expectedReturnedMoney.Euros);
            returnedMoney.Cents.Should().Be(expectedReturnedMoney.Cents);

            _myVendingMachine.Amount.Euros.Should().Be(0);
            _myVendingMachine.Amount.Cents.Should().Be(0);
        }

        [TestMethod]
        public void AddProduct_NewProduct_ShouldSucceed()
        {
            string productName = "New Product";
            Money productPrice = new Money(1, 50); 
            int productCount = 10;

            bool result = _myVendingMachine.AddProduct(productName, productPrice, productCount);

            result.Should().BeTrue();
        }

        [TestMethod]
        public void AddProduct_ExistingProduct_ShouldUpdateProductCount()
        {
            string productName = "Existing Product";
            Money initialPrice = new Money(1, 50);
            int initialCount = 10;

            _myVendingMachine.AddProduct(productName, initialPrice, initialCount);

            int additionalCount = 15;

            _myVendingMachine.AddProduct(productName, initialPrice, additionalCount);

            Product updatedProduct = _myVendingMachine.Products.FirstOrDefault(p => p.Name == productName);

            updatedProduct.Should().NotBeNull();
            updatedProduct.Price.Euros.Should().Be(initialPrice.Euros);
            updatedProduct.Price.Cents.Should().Be(initialPrice.Cents);
            updatedProduct.Available.Should().Be(initialCount + additionalCount);
        }

        [TestMethod]
        public void AddProduct_EmptyName_ShouldThrowInvalidProductNameException()
        {
            string emptyName = "";
            Money productPrice = new Money(1, 50);
            int productCount = 10;

            Action act = () => _myVendingMachine.AddProduct(emptyName, productPrice, productCount);

            act.Should().Throw<InvalidProductNameException>()
                .WithMessage("The product name is not valid");
        }


        [TestMethod]
        public void AddProduct_NegativePrice_ShouldThrowInvalidPriceException()
        {
            string productName = "Product with Negative Price";
            Money negativePrice = new Money(-1, 50); 
            int productCount = 10;

            Action act = () => _myVendingMachine.AddProduct(productName, negativePrice, productCount);

            act.Should().Throw<InvalidProductPriceException>()
                .WithMessage("The price can not be negative");
        }

        [TestMethod]
        public void AddProduct_NegativeCount_ShouldThrowInvalidProductCountException()
        {
            string productName = "Product with Negative Count";
            Money productPrice = new Money(1, 50);
            int negativeCount = -10;

            Action act = () => _myVendingMachine.AddProduct(productName, productPrice, negativeCount);

            act.Should().Throw<InvalidProductCountException>()
                .WithMessage("The product count can not be negative");
        }

        [TestMethod]
        public void UpdateProduct_ExistingProduct_ShouldUpdateDetailsSuccessfully()
        {
            string initialProductName = "Initial Product";
            Money initialPrice = new Money(1, 50);
            int initialCount = 10;
            _myVendingMachine.AddProduct(initialProductName, initialPrice, initialCount);

            string updatedProductName = "Updated Product";
            Money updatedPrice = new Money(2, 0);
            int updatedCount = 20;
            int productNumber = 0; 

            bool updateResult = _myVendingMachine.UpdateProduct(productNumber, updatedProductName, updatedPrice, updatedCount);

            Product updatedProduct = _myVendingMachine.Products[productNumber];

            updateResult.Should().BeTrue();
            updatedProduct.Name.Should().Be(updatedProductName);
            updatedProduct.Price.Euros.Should().Be(updatedPrice.Euros);
            updatedProduct.Price.Cents.Should().Be(updatedPrice.Cents);
            updatedProduct.Available.Should().Be(updatedCount);
        }

        [TestMethod]        
        public void UpdateProduct_NonExistingProduct_ShouldThrowInvalidProductException()
        {
            int nonExistingProductNumber = 999; 
            string newName = "New Name";
            Money newPrice = new Money(2, 0);
            int newAmount = 20;

            Action act = () => _myVendingMachine.UpdateProduct(nonExistingProductNumber, newName, newPrice, newAmount);

            act.Should().Throw<InvalidProductException>()
                .WithMessage("The product is not existing");
        }

        [TestMethod]
        public void UpdateProduct_MissingName_ShouldThrowInvalidProductNameException()
        {
            string initialProductName = "Initial Product";
            Money initialPrice = new Money(1, 50);
            int initialCount = 10;
            _myVendingMachine.AddProduct(initialProductName, initialPrice, initialCount);

            string emptyName = ""; 
            int productNumber = 0; 

            Action act = () => _myVendingMachine.UpdateProduct(productNumber, emptyName, null, initialCount);

            act.Should().Throw<InvalidProductNameException>()
                .WithMessage("The product name is not valid");
        }

        [TestMethod]
        public void UpdateProduct_NegativePrice_ShouldThrowInvalidPriceException()
        {
            string initialProductName = "Initial Product";
            Money initialPrice = new Money(1, 50);
            int initialCount = 10;
            _myVendingMachine.AddProduct(initialProductName, initialPrice, initialCount);

            int productNumber = 0; 
            Money negativePrice = new Money(-1, 0);

            Action act = () => _myVendingMachine.UpdateProduct(productNumber, initialProductName, negativePrice, initialCount);

            act.Should().Throw<InvalidProductPriceException>()
                .WithMessage("The price can not be negative");
        }

        [TestMethod]
        public void UpdateProduct_NegativeCount_ShouldThrowInvalidProductCountException()
{
            string initialProductName = "Initial Product";
            Money initialPrice = new Money(1, 50);
            int initialCount = 10;
            _myVendingMachine.AddProduct(initialProductName, initialPrice, initialCount);

            int productNumber = 0;
            int negativeCount = -5;

            Action act = () => _myVendingMachine.UpdateProduct(productNumber, initialProductName, initialPrice, negativeCount);

            act.Should().Throw<InvalidProductCountException>()
                .WithMessage("The product count can not be negative");
        }

        [TestCleanup]
        public void Cleanup()
        {
            _myVendingMachine = null; 
        }
    }
}
