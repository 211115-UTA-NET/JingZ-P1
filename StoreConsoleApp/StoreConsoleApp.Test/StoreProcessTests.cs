using Microsoft.AspNetCore.WebUtilities;
using Moq;
using StoreConsoleApp.UI;
using StoreConsoleApp.UI.Dtos;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace StoreConsoleApp.Test
{
    public class StoreProcessTests
    {
        [Fact]
        public async Task TestGetLocationAsync()
        {
            Mock<IRequestServices> mockService = new();
            List<Location> locations = new()
            {
                new()
                {
                    LocationID=1,
                    StoreLocation="1551 3rd Ave, New York, NY 10128"
                },
                new()
                {
                    LocationID = 2,
                    StoreLocation = "367 Russell St, Hadley, MA 01035"
                },
                new()
                {
                    LocationID = 3,
                    StoreLocation = "613 Washington Blvd, Jersey City, NJ 073108"
                }
            };
            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent(JsonSerializer.Serialize(locations), Encoding.UTF8, MediaTypeNames.Application.Json);
            mockService.Setup(x => x.GetResponseForGETAsync("/api/storeinfo")).Returns(Task.FromResult(responseMessage));
            var store = new StoreProcess(mockService.Object);
            string result = await store.GetLocationsAsync();
            var expected = "ID\tStore Location\r\n---------------------------------------------------------------\r\n1\t[1551 3rd Ave, New York, NY 10128]\r\n2\t[367 Russell St, Hadley, MA 01035]\r\n3\t[613 Washington Blvd, Jersey City, NJ 073108]\r\n---------------------------------------------------------------\r\n";
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("0")] // invalid location id
        [InlineData("3")] // valid location id
        public async Task TestGetStoreProductAsync(string locationID)
        {
            string requestUri = $"/api/storeinfo/{locationID}";
            Mock<IRequestServices> mockService = new();
            HttpResponseMessage responseMessage = new();
            if (locationID == "0")
            {
                responseMessage.Content = new StringContent("[]", Encoding.UTF8, MediaTypeNames.Application.Json);
                mockService.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));
            }
            else
            {
                List<Product> products = new()
                {
                    new()
                    {
                       ProductName= "Masking Tape",
                       Price = (decimal)1.9
                    },
                    new()
                    {
                        ProductName = "Sticky Index",
                        Price = (decimal)2.9
                    },
                    new()
                    {
                        ProductName = "Stapler",
                        Price = (decimal)4.9
                    },
                    new()
                    {
                        ProductName = "Tape Dispenser",
                        Price = (decimal)9.9
                    },
                    new()
                    {
                        ProductName = "Standard File Box",
                        Price = (decimal)10.9
                    }
                };
                responseMessage.Content = new StringContent(JsonSerializer.Serialize(products), Encoding.UTF8, MediaTypeNames.Application.Json);
                mockService.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));
            }
            var store = new StoreProcess(mockService.Object);
            var getStoreProducts = await store.GetStoreProductsAsync(locationID);
            string result = getStoreProducts.Item1;
            bool validId = getStoreProducts.Item2;
            if (locationID == "0")
            {
                var expected = "--- Your Input is invalid, please try again. ---\r\n";
                Assert.Equal(expected, result);
                Assert.False(validId);
            }
            else
            {
                var expected = "ID\t\tProduct Name\t\t\tPrice\r\n---------------------------------------------------------------\r\n    1 |                   Masking Tape |        1.9\r\n    2 |                   Sticky Index |        2.9\r\n    3 |                        Stapler |        4.9\r\n    4 |                 Tape Dispenser |        9.9\r\n    5 |              Standard File Box |       10.9\r\n---------------------------------------------------------------\r\n";
                Assert.Equal(expected, result);
                Assert.True(validId);
            }
        }

        [Theory]
        [InlineData("Mike", "James")]
        public async Task TestCreateAccountAsync(string firstName, string lastName)
        {
            Mock<IRequestServices> mockService = new();
            CustomerName customerName = new()
            {
                firstName = firstName,
                lastName = lastName
            };
            string json = JsonSerializer.Serialize(customerName);
            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent("109", Encoding.UTF8, MediaTypeNames.Application.Json);
            mockService.Setup(x => x.GetResponseForPOSTAsync(json, "/api/customer")).Returns(Task.FromResult(responseMessage));
            var store = new StoreProcess(mockService.Object);
            int result = await store.CreateAccount(firstName, lastName);
            Assert.Equal(109, result);
        }

        [Theory]
        [InlineData("9")] // invalid customer id
        [InlineData("abc")] // invalid customer id, must be number
        public async Task TestSearchCustomerInvalidParamsAsync(string customerID, string firstName = "", string lastName = "")
        {
            Mock<IRequestServices> mockService = new();

            Dictionary<string, string> query = new() { ["CustomerID"] = customerID, ["LastName"] = lastName, ["FirstName"] = firstName };
            string requestUri = QueryHelpers.AddQueryString("/api/customer/login", query);

            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent("[]", Encoding.UTF8, MediaTypeNames.Application.Json);

            mockService.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));

            var store = new StoreProcess(mockService.Object);
            var searchCustomer = await store.SearchCustomerAsync(customerID, firstName, lastName);
            bool result = searchCustomer.Item1;
            int CustomerID = searchCustomer.Item2;
            Assert.False(result);
        }

        [Theory]
        [InlineData("106")] // Valid customer id
        [InlineData("forgot", "Mike", "James")]  // customer id = "forgot", and name is provided
        public async Task TestSearchCustomerValidCustomerIDAsync(string customerID, string firstName = "", string lastName = "")
        {
            Mock<IRequestServices> mockService = new();
            List<Customer> customer = new()
            {
                new()
                {
                    CustomerID = 106,
                    FirstName = "Nancy",
                    LastName = "Smith"
                }
            };
            Dictionary<string, string> query = new() { ["CustomerID"] = customerID, ["LastName"] = lastName, ["FirstName"] = firstName };
            string requestUri = QueryHelpers.AddQueryString("/api/customer/login", query);

            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent(JsonSerializer.Serialize(customer), Encoding.UTF8, MediaTypeNames.Application.Json);

            mockService.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));
            var store = new StoreProcess(mockService.Object);
            var searchCustomer = await store.SearchCustomerAsync(customerID, firstName, lastName);
            bool result = searchCustomer.Item1;
            int CustomerID = searchCustomer.Item2;
            Assert.True(result);
        }

        [Theory]
        [InlineData("999")]
        [InlineData("-1")]
        [InlineData("abc")]
        public void TestValidProductIDInvalidProductID(string productID)
        {
            Mock<IRequestServices> mockService = new();
            var store = new StoreProcess(mockService.Object);

            bool result = store.ValidProductID(productID, out string productName);
            Assert.False(result);
            Assert.Equal("", productName);
        }

        [Fact]
        public async Task TestValidProductIDValidProductIDAsync()
        {
            Mock<IRequestServices> mockService = new();
            List<Product> products = new()
            {
                new()
                {
                    ProductName = "Masking Tape",
                    Price = (decimal)1.9
                },
                new()
                {
                    ProductName = "Sticky Index",
                    Price = (decimal)2.9
                },
                new()
                {
                    ProductName = "Stapler",
                    Price = (decimal)4.9
                },
                new()
                {
                    ProductName = "Tape Dispenser",
                    Price = (decimal)9.9
                },
                new()
                {
                    ProductName = "Standard File Box",
                    Price = (decimal)10.9
                }
            };
            string requestUri = $"/api/storeinfo/3";
            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent(JsonSerializer.Serialize(products), Encoding.UTF8, MediaTypeNames.Application.Json);
            mockService.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));
            var store = new StoreProcess(mockService.Object);
            var tmp = await store.GetStoreProductsAsync("3");  // initialize private field: ProductList
            bool result = store.ValidProductID("1", out string ProductName);
            Assert.True(result);
            Assert.Equal(products[0].ProductName, ProductName);
        }

        [Theory]
        [InlineData("Masking Tape", "amount", 3)]
        [InlineData("Masking Tape", "0", 3)]
        [InlineData("Masking Tape", "100", 3)]
        [InlineData("Masking Tape", "247", 3)]
        public async Task TestValidAmountInvalidAmountAsync(string productName, string amount, int locationID)
        {
            Mock<IRequestServices> mockRepo = new();
            Dictionary<string, string> query = new() { ["productName"] = productName, ["locationID"] = locationID + "" };
            string requestUri = QueryHelpers.AddQueryString("/api/Order/inventory", query);

            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent(JsonSerializer.Serialize("246"), Encoding.UTF8, MediaTypeNames.Application.Json);

            mockRepo.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));
            var store = new StoreProcess(mockRepo.Object);
            var amountResult = await store.validAmountAsync(productName, amount, locationID);
            bool result = amountResult.Item1;
            Assert.False(result);
        }

        [Theory]
        [InlineData("Masking Tape", "3", 3)]
        public async Task TestValidAmountValidAmountAsync(string productName, string amount, int locationID)
        {
            Mock<IRequestServices> mockRepo = new();

            Dictionary<string, string> query = new() { ["productName"] = productName, ["locationID"] = locationID + "" };
            string requestUri = QueryHelpers.AddQueryString("/api/Order/inventory", query);

            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent(JsonSerializer.Serialize("246"), Encoding.UTF8, MediaTypeNames.Application.Json);
            
            mockRepo.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));
            var store = new StoreProcess(mockRepo.Object);
            var amountResult = await store.validAmountAsync(productName, amount, locationID);
            bool result = amountResult.Item1;
            Assert.True(result);
        }
    }
}