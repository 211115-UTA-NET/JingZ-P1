using Microsoft.AspNetCore.WebUtilities;
using StoreConsoleApp.UI.Dtos;
using StoreConsoleApp.UI.Exceptions;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace StoreConsoleApp.UI
{
    public class OrderProcess
    {
        private readonly IRequestServices service;

        public OrderProcess(IRequestServices requestServices)
        {
            service = requestServices;
        }

        /// <summary>
        ///     Used to display the order detail when a user palced an order. 
        ///     Process include insert order information to database then display the result.
        /// </summary>
        /// <param name="customerID">customer id number</param>
        /// <param name="productNames">selected products as a list</param>
        /// <param name="productQty">selected product quantities as a list</param>
        /// <param name="locationID">store location id</param>
        /// <returns>A string of the order receipt, and a bool checks if process succeed, false otherwise.</returns>
        public async Task<(string, bool)> DisplayOrderDetail(StoreProcess store, int customerID, List<string> productNames, List<int> productQty, int locationID)
        {
            bool Processfailed;
            var receipt = new StringBuilder();
            Dictionary<string, string> query = new() { ["customerID"] = customerID+"" };
            string requestUri = QueryHelpers.AddQueryString("/api/order/ordernum", query);
            var response = await service.GetResponseForGETAsync(requestUri);
            int orderNumber = await response.Content.ReadFromJsonAsync<int>();
            List<Order> order = new();
            if (orderNumber < 0)
            {
                Console.WriteLine("--- For some reason, unable to process your order. ---");
                Processfailed = true;
            }
            else
            {
                for (int i = 0; i < productNames.Count; i++)
                {
                    // price does not matters here
                    order.Add(new()
                    {
                        OrderNum = orderNumber,
                        ProductName = productNames[i],
                        ProductQty = productQty[i],
                        LocationID = locationID
                    });
                }
                // add order to database, return summary
                IEnumerable<Order> allRecords = await ProcessOrder(order);
                if (allRecords == null || !allRecords.Any())
                {
                    receipt.AppendLine("--- Your Input is invalid, please try again. ---");
                    Processfailed = true;
                }
                else
                {
                    receipt.AppendLine(await OrderRecordFormatAsync(locationID, allRecords));
                    Processfailed = false;
                }
            }
            return (receipt.ToString(), Processfailed);
        }

        /// <summary>
        ///     Used to display order history. Process params value to get the information back.
        ///     If locationID param is provided then it will return order history of the user in current store location.
        ///     If OrderNum param is provided then it will return order details of a specific/most recent order.
        ///     else if locationID and OrderNum params both not provided, then it will return all orders of this customer.
        /// </summary>
        /// <param name="customerID">customer id</param>
        /// <param name="locationID">(optional param) location ID provided for search local order history</param>
        /// <param name="OrderNum">(optional param) sepecific order number or [0] for most recent. provided for searching specific/most recent order</param>
        /// <returns>A string of order history based on user input params. A bool checks true when get order history failed, return false otherwise. See summary for details.</returns>
        public async Task<(string, bool)> DisplayOrderHistory(int customerID, int locationID = -1, int OrderNum = -1)
        {
            bool getHistoryFailed;
            var orderHistory = new StringBuilder();
            string requestUri;
            if (locationID > -1 && OrderNum < 0)
            {
                // get all order history from the local store
                Dictionary<string, string> query = new() { ["customerId"] = customerID + "", ["locationId"] = locationID+"" };
                requestUri = QueryHelpers.AddQueryString("/api/order/local", query);

            }
            else if(OrderNum > -1 && locationID < 0)
            {
                // get most recent order
                if(OrderNum == 0)
                {
                    Dictionary<string, string> query = new() { ["customerId"] = customerID+""};
                    requestUri = QueryHelpers.AddQueryString("/api/order/recent", query);
                }
                else
                {
                    // get order by order number
                    Dictionary<string, string> query = new() { ["customerId"] = customerID + "", ["orderNum"] = OrderNum+"" };
                    requestUri = QueryHelpers.AddQueryString("/api/order/specific", query);
                }
            }
            else
            {
                // get all order history of the customer
                Dictionary<string, string> query = new() { ["customerId"] = customerID + "" };
                requestUri = QueryHelpers.AddQueryString("/api/order/all", query);
            }
            // send request to service
            var response = await service.GetResponseForGETAsync(requestUri);
            var allRecords = await response.Content.ReadFromJsonAsync<List<Order>>();
            // process allRecords
            if (allRecords == null || !allRecords.Any())
            {
                orderHistory.AppendLine("--- Your order histroy is empty. ---");
                getHistoryFailed = true;
            }
            else
            {
                List<Order> tmp = new();
                int prevOrderNum = allRecords[0].OrderNum;
                tmp.Add(allRecords[0]);
                if (allRecords.Count == 1)
                {
                    orderHistory.AppendLine(await OrderRecordFormatAsync(allRecords[0].LocationID, allRecords));
                }
                else
                {
                    // formatting the order info: same order# goes to same block of format, else start a new block of format
                    int currentOrderNum;
                    for (int i = 1; i < allRecords.Count; i++)
                    {
                        currentOrderNum = allRecords[i].OrderNum;
                        if (currentOrderNum == prevOrderNum)
                        {
                            tmp.Add(allRecords[i]);
                            if (i == allRecords.Count - 1)
                                orderHistory.AppendLine(await OrderRecordFormatAsync(allRecords[i].LocationID, tmp));
                        }
                        else
                        {
                            orderHistory.AppendLine(await OrderRecordFormatAsync(allRecords[i-1].LocationID, tmp));
                            tmp = new();
                            tmp.Add(allRecords[i]);
                            // if last records didn't append
                            if (i == allRecords.Count - 1)
                                orderHistory.AppendLine(await OrderRecordFormatAsync(allRecords[i].LocationID, tmp));
                        }
                        prevOrderNum = currentOrderNum;
                    }
                }
                getHistoryFailed = false;
            }
            return (orderHistory.ToString(), getHistoryFailed);
        }
        
        
        /// <summary>
        ///     A display format for the order history.
        /// </summary>
        /// <param name="allRecords">Order class type collection</param>
        /// <returns>A string of formated order history.</returns>
        private async Task<string> OrderRecordFormatAsync(int locationId, IEnumerable<Order> allRecords)
        {
            // get product price
            StoreProcess storeProcess = new(service);
            var products = await storeProcess.GetProductList(locationId);
            var price = storeProcess.GetProductPrice((List<Order>)allRecords, products);
            if(price == null)
            {
                throw new UnexpectedServerBehaviorException();
            }
            var format = new StringBuilder();
            int once = 0;
            int i = 0;
            decimal totalPrice = 0;
            foreach (var record in allRecords)
            {
                if (once == 0)
                {
                    format.AppendLine(string.Format("Order#: {0} | Ordered at: {1,10}", record.OrderNum, record.OrderTime));
                    format.AppendLine("Store Location: " + record.Location);
                    format.AppendLine($"Product Name\t\t\tPurchased Quantity\tPrice");
                    format.AppendLine("---------------------------------------------------------------");
                    once++;
                }
                price[i] *= record.ProductQty;
                try
                {
                    format.AppendLine(string.Format("{0,30} | {1,13} | {2,10}", record.ProductName, record.ProductQty, price[i]));
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                totalPrice += price[i];
                i++;
            }
            format.AppendLine("---------------------------------------------------------------");
            format.AppendLine($"Total Price: ${totalPrice}\n");
            return format.ToString();
        }


        /// <summary>
        ///     Used in DisplayOrderDetail() method.
        ///     Call repository to insert the order to database. Return inserted order information.
        /// </summary>
        /// <param name="orders">Order type list</param>
        /// <returns>Inserted order information</returns>
        private async Task<IEnumerable<Order>> ProcessOrder(List<Order> orders)
        {
            OrderList list = new();
            list.orderlist = orders;
            var json = JsonSerializer.Serialize(list);
            var response = await service.GetResponseForPOSTAsync(json, "/api/order");
            var allRecords = await response.Content.ReadFromJsonAsync<List<Order>>();
            if (allRecords == null)
            {
                throw new UnexpectedServerBehaviorException();
            }
            return allRecords;
        }
    }
}
