using Microsoft.AspNetCore.WebUtilities;
using StoreConsoleApp.UI.Dtos;
using StoreConsoleApp.UI.Exceptions;
using System.Net.Http.Json;
using System.Text;

namespace StoreConsoleApp.UI
{
    public class StoreProcess
    {
        /// <summary>
        ///     store the displayed product list of the store location.
        /// </summary>
        private List<Product> ProductList;
        RequestServices service = new();
        public StoreProcess()
        {
            ProductList = new();
        }

        /// <summary>
        ///     Get location id and store location from database
        /// </summary>
        /// <returns>A string of formated store location list</returns>
        public async Task<string> GetLocationsAsync()
        {
            HttpResponseMessage response = await service.GetResponseAsync("/api/storeinfo");

            // store response in dto
            var allRecords = await response.Content.ReadFromJsonAsync<List<Location>>();
            if (allRecords == null)
            {
                throw new UnexpectedServerBehaviorException();
            }
            var locations = new StringBuilder();
            locations.AppendLine($"ID\tStore Location");
            locations.AppendLine("---------------------------------------------------------------");
            foreach (var record in allRecords)
            {
                locations.AppendLine($"{record.LocationID}\t[{record.StoreLocation}]");
            }
            locations.AppendLine("---------------------------------------------------------------");
            // get location always call before get store product.
            // So product list will initialize each time user want to switch a location
            ProductList = new(); 
            return locations.ToString();
        }

        /// <summary>
        ///     Display selected store location products.
        /// </summary>
        /// <param name="locationID">location id</param>
        /// <returns>A string of formated store location list, and bool value determine validID</returns>
        public async Task<(string, bool)> GetStoreProductsAsync(string locationID)
        {
            ProductList = new();
            bool validID;
            string requestUri = $"/api/storeinfo/{locationID}";
            HttpResponseMessage response = await service.GetResponseAsync(requestUri);

            var allRecords = await response.Content.ReadFromJsonAsync<List<Product>>();
            var products = new StringBuilder();
            if (allRecords == null || !allRecords.Any())
            {
                products.AppendLine("--- Your Input is invalid, please try again. ---");
                validID = false;
            }
            else
            {
                validID = true;
                products.AppendLine($"ID\t\tProduct Name\t\t\tPrice");
                products.AppendLine("---------------------------------------------------------------");
                int i = 1;
                foreach (var record in allRecords)
                {
                    // store ProductName
                    ProductList.Add(record);
                    products.AppendLine(string.Format("{0,5} | {1,30} | {2,10}", i, record.ProductName, record.Price));
                    i++;
                }
                products.AppendLine("---------------------------------------------------------------");
            }
            
            return (products.ToString(), validID);
        }

        /// <summary>
        ///     Added new customer to the database
        /// </summary>
        /// <param name="firstName">new customer first name</param>
        /// <param name="lastName">new customer last name</param>
        /// <returns>Customer ID</returns>
        public async Task<int> CreateAccount(string firstName, string lastName)
        {
            CustomerName customerName = new()
            {
                firstName = firstName,
                lastName = lastName
            };
            var response = await service.GetResponseForAddCustomerAsync(customerName);
            int CustomerID = await response.Content.ReadFromJsonAsync<int>();
            return CustomerID;
        }

        /// <summary>
        ///     Search customer by customer ID or customer name.
        ///     if CustomerID = 'forgot', then it will search customer by name.
        ///     else will search by CustomerID.
        /// </summary>
        /// <param name="customerID">customer ID</param>
        /// <param name="firstName">customer first name</param>
        /// <param name="lastName">customer last name</param>
        /// <returns>true if customer exists, false otherwise. And customer id</returns>
        public async Task<(bool, int)> SearchCustomerAsync(string customerID, string firstName = "", string lastName = "")
        {
            Dictionary<string, string> query = new() { ["CustomerID"] = customerID, ["LastName"] = lastName, ["FirstName"] = firstName };
            string requestUri = QueryHelpers.AddQueryString("/api/customer/login", query);
            var response = await service.GetResponseAsync(requestUri);
            var customer = await response.Content.ReadFromJsonAsync<List<Customer>>();
            int CustomerID = -1;  //second return value
            if (customer == null || !customer.Any())
            {
                Console.WriteLine("--- Account Not Found. Please Try Again. ---");
                return (false, CustomerID);
            }
            foreach (var existCustomer in customer)
            {
                Console.WriteLine($"\nWelcome Back! {existCustomer.FirstName} {existCustomer.LastName}.\n" +
                    $"Please Remember Your Customer ID#: {existCustomer.CustomerID}\n");
                CustomerID = existCustomer.CustomerID;
            }
            return (true, CustomerID);
        }

        /// <summary>
        ///     Check is product name exists in the ProductList using productID as index.
        /// </summary>
        /// <param name="productID">Product ID</param>
        /// <param name="ProductName">return the product name</param>
        /// <returns>true if product id is valid, false otherwise</returns>
        public bool ValidProductID(string productID, out string ProductName)
        {
            if (int.TryParse(productID, out int productId))
            {
                if (productId > 0 && productId <= ProductList.Count) // must within array range
                {
                    productId -= 1; // used as List index
                    if (productId >= 0 && ProductList.Count > productId)
                    {
                        ProductName = ProductList[productId].ProductName!;
                        return true;
                    }
                }
            }
            ProductName = "";
            return false;
        }

        public List<decimal> GetProductPrice(List<Order> allRecords)
        {
            List<decimal> prices = new();
            foreach(var record in allRecords)
            {
                prices.Add(FindPrice(record.ProductName!));
            }
            return prices;
        }

        private decimal FindPrice(string productName)
        {
            foreach (var product in ProductList)
            {
                if(product.ProductName == productName)
                    return product.Price;
            }
            throw new ProductNotFoundException();
        }

        /// <summary>
        ///     Check if user select product quantity is valid or not by comparing the store inventory amount
        /// </summary>
        /// <param name="productName">User selected product (valid)</param>
        /// <param name="amount">User input amount</param>
        /// <param name="locationID">Valid location ID</param>
        /// <param name="orderAmount">return the valid order amount</param>
        /// <returns>true if amount is valid, false otherwise.</returns>
        public async Task<(bool, int)> validAmountAsync(string productName, string amount, int locationID)
        {
            int orderAmount;
            // amount <= inventory amount
            if (int.TryParse(amount, out orderAmount))
            {
                if (orderAmount >= 100 || orderAmount <= 0)
                {
                    Console.WriteLine("\n--- Quantity cannot be 0 and cannot exceed the Max limit. ---");
                    return (false, orderAmount);
                } // cannot order more than 99
                Dictionary<string, string> query = new() { ["productName"] = productName, ["locationID"] = locationID+"" };
                string requestUri = QueryHelpers.AddQueryString("/api/Order/inventory", query);
                var response = await service.GetResponseAsync(requestUri);
                int inventoryAmount = await response.Content.ReadFromJsonAsync<int>();
                // Console.WriteLine("inventory amount: " + inventoryAmount);
                if (orderAmount <= inventoryAmount)
                {
                    return (true, orderAmount);
                }
                else Console.WriteLine("\n--- Sorry, this product is OUT of STOCK... Please select another product. ---");
            }
            return (false, orderAmount);
        }
    }
}
