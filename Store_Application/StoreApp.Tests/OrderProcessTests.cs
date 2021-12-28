using Moq;
using StoreApp.App;
using StoreApp.DataInfrastructure;
using StoreApp.Logic;
using System;
using System.Collections.Generic;
using Xunit;

namespace StoreApp.Tests
{
    public class OrderProcessTests
    {
        [Theory]
        [InlineData(-1)]  // Invalid customer ID
        [InlineData(106, 0)]  // valid customer ID, Invalid location ID
        [InlineData(106, -1, 1000)] // valid customer ID, Invalid orderNum
        public void DisplayAllOrderHistoryInvalidParameters(int customerID, int locationID =-1, int orderNum = -1)
        {
            Mock<IRepository> mockRepo = new();
            // lambda expression syntax: like an anonymous classless method (this kind of value in C# is called a "delegate")
            // the Mock class sets up its inner object using these method calls (Setup, Returns) and some magic called "reflection"
            mockRepo.Setup(x => x.GetStoreOrders(customerID)).Returns(new List<Order>());
            mockRepo.Setup(x => x.GetLocationOrders(customerID, locationID)).Returns(new List<Order>());
            mockRepo.Setup(x => x.GetMostRecentOrder(customerID)).Returns(new List<Order>());
            mockRepo.Setup(x => x.GetSpecificOrder(customerID, orderNum)).Returns(new List<Order>());
            var orderProcess = new OrderProcess(mockRepo.Object);
            string result = orderProcess.DisplayOrderHistory(customerID, out bool failed, locationID, orderNum);
            var expected = "--- Your order histroy is empty. ---\r\n";
            Assert.Equal(expected, result);
            Assert.True(failed);
        }


    }
}