using Moq;
using System.Collections.Generic;
using Xunit;
using StoreConsoleApp.UI;
using StoreConsoleApp.UI.Dtos;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace StoreConsoleApp.Test
{
    public class OrderProcessTests
    {
        [Theory]
        [InlineData(106, 0)]    // valid customer ID, Invalid location ID
        [InlineData(100, 3)]    // invalid customer ID, valid loaction ID
        public async Task DisplayLocalOrderHistoryInvalidParameters(int customerID, int locationID = -1, int orderNum = -1)
        {
            Mock<IRequestServices> mockService = new();
            // test local order history
            Dictionary<string, string> query = new() { ["customerId"] = customerID + "", ["locationId"] = locationID + "" };
            string requestUri = QueryHelpers.AddQueryString("/api/order/local", query);

            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent("[]", Encoding.UTF8, MediaTypeNames.Application.Json);

            mockService.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));

            var orderProcess = new OrderProcess(mockService.Object);
            var orderResult = await orderProcess.DisplayOrderHistory(customerID, locationID, orderNum);
            string result = orderResult.Item1;
            bool failed = orderResult.Item2;
            var expected = "--- Your order histroy is empty. ---\r\n";
            Assert.Equal(expected, result);
            Assert.True(failed);
        }

        [Theory]
        [InlineData(100)]    // invalid customer ID
        public async Task DisplayAllOrderHistoryInvalidParameters(int customerID, int locationID = -1, int orderNum = -1)
        {
            Mock<IRequestServices> mockService = new();
            // test local order history
            Dictionary<string, string> query = new() { ["customerId"] = customerID + "" };
            string requestUri = QueryHelpers.AddQueryString("/api/order/all", query);

            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent("[]", Encoding.UTF8, MediaTypeNames.Application.Json);

            mockService.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));

            var orderProcess = new OrderProcess(mockService.Object);
            var orderResult = await orderProcess.DisplayOrderHistory(customerID, locationID, orderNum);
            string result = orderResult.Item1;
            bool failed = orderResult.Item2;
            var expected = "--- Your order histroy is empty. ---\r\n";
            Assert.Equal(expected, result);
            Assert.True(failed);
        }

        [Theory]
        [InlineData(100, -1, 1)]    // invalid customer ID
        public async Task DisplaySpecificOrderHistoryInvalidParameters(int customerID, int locationID = -1, int orderNum = -1)
        {
            Mock<IRequestServices> mockService = new();
            // test local order history
            Dictionary<string, string> query = new() { ["customerId"] = customerID + "", ["orderNum"] = orderNum + "" };
            string requestUri = QueryHelpers.AddQueryString("/api/order/specific", query);

            HttpResponseMessage responseMessage = new();
            responseMessage.Content = new StringContent("[]", Encoding.UTF8, MediaTypeNames.Application.Json);

            mockService.Setup(x => x.GetResponseForGETAsync(requestUri)).Returns(Task.FromResult(responseMessage));

            var orderProcess = new OrderProcess(mockService.Object);
            var orderResult = await orderProcess.DisplayOrderHistory(customerID, locationID, orderNum);
            string result = orderResult.Item1;
            bool failed = orderResult.Item2;
            var expected = "--- Your order histroy is empty. ---\r\n";
            Assert.Equal(expected, result);
            Assert.True(failed);
        }
    }
}
