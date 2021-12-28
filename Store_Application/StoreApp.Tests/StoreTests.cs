using Moq;
using StoreApp.App;
using StoreApp.DataInfrastructure;
using StoreApp.Logic;
using System;
using System.Collections.Generic;
using Xunit;

namespace StoreApp.Tests
{
    public class StoreTests
    {
        [Fact]
        public void TestGetLocation()
        {
            Mock<IRepository> mockRepo = new();
            List<Location> locations = new ()
            {
                new (1, "1551 3rd Ave, New York, NY 10128"),
                new (2, "367 Russell St, Hadley, MA 01035"),
                new (3, "613 Washington Blvd, Jersey City, NJ 07310")
            };
            mockRepo.Setup(x => x.GetLocationList()).Returns(locations);
            var store = new Store(mockRepo.Object);
            string result = store.GetLocations();
            var expected = "ID\tStore Location\r\n---------------------------------------------------------------\r\n1\t[1551 3rd Ave, New York, NY 10128]\r\n2\t[367 Russell St, Hadley, MA 01035]\r\n3\t[613 Washington Blvd, Jersey City, NJ 07310]\r\n---------------------------------------------------------------\r\n";
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("0")] // invalid location id
        [InlineData("3")] // valid location id
        public void TestGetStoreProduct(string locationID)
        {
            Mock<IRepository> mockRepo = new();
            if (locationID == "0") {
                mockRepo.Setup(x => x.GetStoreProducts(locationID)).Returns(new List<Product>());
            } else {
                List<Product> products = new()
                {
                    new("Masking Tape", (decimal)1.9),
                    new("Sticky Index", (decimal)2.9),
                    new("Stapler", (decimal)4.9),
                    new("Tape Dispenser", (decimal)9.9),
                    new("Standard File Box", (decimal)10.9)
                };
                mockRepo.Setup(x => x.GetStoreProducts(locationID)).Returns(products);
            }
            var store = new Store(mockRepo.Object);
            string result = store.GetStoreProducts(locationID, out bool validID);
            if (locationID == "0")
            {
                var expected = "--- Your Input is invalid, please try again. ---\r\n";
                Assert.Equal(expected, result);
                Assert.False(validID);
            }
            else
            {
                var expected = "ID\t\tProduct Name\t\t\tPrice\r\n---------------------------------------------------------------\r\n    1 |                   Masking Tape |        1.9\r\n    2 |                   Sticky Index |        2.9\r\n    3 |                        Stapler |        4.9\r\n    4 |                 Tape Dispenser |        9.9\r\n    5 |              Standard File Box |       10.9\r\n---------------------------------------------------------------\r\n";
                Assert.Equal(expected, result);
                Assert.True(validID);
            }
        }

        [Theory]
        [InlineData("Mike", "James")]
        public void TestCreateAccount(string firstName, string lastName)
        {
            Mock<IRepository> mockRepo = new();
            mockRepo.Setup(x => x.AddNewCustomer(firstName, lastName)).Returns(109);
            var store = new Store(mockRepo.Object);
            int result = store.CreateAccount(firstName, lastName);
            Assert.Equal(109, result);
        }

        [Theory]
        [InlineData("9")] // invalid customer id
        [InlineData("abc")] // invalid customer id, must be number
        public void TestSearchCustomerInvalidParams(string customerID, string firstName = "", string lastName = "") 
        {
            Mock<IRepository> mockRepo = new();
            mockRepo.Setup(x => x.FindCustomer(customerID, firstName, lastName)).Returns(new List<Customer>());
            var store = new Store(mockRepo.Object);
            bool result = store.SearchCustomer(customerID, out int CustomerID, firstName, lastName);
            Assert.False(result);
        }

        [Theory]
        [InlineData("106")] // Valid customer id
        [InlineData("forgot", "Mike", "James")]  // customer id = "forgot", and name is provided
        public void TestSearchCustomerValidCustomerID(string customerID, string firstName = "", string lastName = "")
        {
            Mock<IRepository> mockRepo = new();
            List<Customer> customer = new()
            {
                new(106, "Nancy", "Smith")
            };
            mockRepo.Setup(x => x.FindCustomer(customerID, firstName, lastName)).Returns(customer);
            var store = new Store(mockRepo.Object);
            bool result = store.SearchCustomer(customerID, out int CustomerID, firstName, lastName);
            Assert.True(result);
        }

        [Theory]
        [InlineData("999")]
        [InlineData("-1")]
        [InlineData("abc")]
        public void TestValidProductIDInvalidProductID(string productID)
        {
            Mock<IRepository> mockRepo = new();
            var store = new Store(mockRepo.Object);

            bool result = store.ValidProductID(productID, out string productName);
            Assert.False(result);
            Assert.Equal("", productName);
        }

        [Fact]
        public void TestValidProductIDValidProductID()
        {
            Mock<IRepository> mockRepo = new();
            List<Product> products = new()
            {
                new("Masking Tape", (decimal)1.9),
                new("Sticky Index", (decimal)2.9),
                new("Stapler", (decimal)4.9),
                new("Tape Dispenser", (decimal)9.9),
                new("Standard File Box", (decimal)10.9)
            };
            mockRepo.Setup(x => x.GetStoreProducts("3")).Returns(products);
            var store = new Store(mockRepo.Object);
            string tmp = store.GetStoreProducts("3", out bool validID); // initialize private field: ProductList
            bool result = store.ValidProductID( "1", out string ProductName);
            Assert.True(result);
            Assert.Equal(products[0].ProductName, ProductName);
        }

        [Theory]
        [InlineData("Masking Tape", "amount", 3)]
        [InlineData("Masking Tape", "0", 3)]
        [InlineData("Masking Tape", "100", 3)]
        [InlineData("Masking Tape", "247", 3)]
        public void TestValidAmountInvalidAmount(string productName, string amount, int locationID)
        {
            Mock<IRepository> mockRepo = new();
            mockRepo.Setup(x => x.InventoryAmount(productName, locationID)).Returns(246);
            var store = new Store(mockRepo.Object);
            bool result = store.validAmount(productName, amount, locationID, out int orderAmount);
            Assert.False(result);
        }

        [Theory]
        [InlineData("Masking Tape", "3", 3)]
        public void TestValidAmountValidAmount(string productName, string amount, int locationID)
        {
            Mock<IRepository> mockRepo = new();
            mockRepo.Setup(x => x.InventoryAmount(productName, locationID)).Returns(246);
            var store = new Store(mockRepo.Object);
            bool result = store.validAmount(productName, amount, locationID, out int orderAmount);
            Assert.True(result);
        }
    }
}
