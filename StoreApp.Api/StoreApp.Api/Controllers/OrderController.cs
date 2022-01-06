using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StoreApi.DataStorage;
using StoreApi.Logic;
using StoreApp.Api.Dtos;
using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger<OrderController> _logger;
        public OrderController(IRepository repository, ILogger<OrderController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // POST api/order
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Order>>> PlaceOrderAsync([FromBody, Required] OrderList orders)
        {
            IEnumerable<Order> orderInfo;
            List<Order> orderList = new();
            foreach(OrderInfo order in orders.orderlist!)
            {
                _logger.LogInformation("Order added to list");
                orderList.Add(new(order.OrderNum, order.ProductName!, order.ProductQty, order.LocationID, order.OrderTime));
            }
            try
            {
                _logger.LogInformation("*** [POST] place an order ***");
                orderInfo = await _repository.AddOrderAsync(orderList);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "*** SQL ERROR! Unable to [POST] place an order... ***");
                return StatusCode(500);
            }

            return orderInfo.ToList();
        }

        /// <summary>
        /// Add a record of customer placed a order then return the order number
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns></returns>
        // GET api/order/ordernum?customerID={id}
        [HttpGet("ordernum")]
        public async Task<ActionResult<int>> GetOrderNumberAsync([FromQuery, Required] string customerID)
        {
            int orderNum;
            try
            {
                _logger.LogInformation("*** [GET] customer Id {customerID} recent order # ***", customerID);
                orderNum = await _repository.GetOrderNumberAsync(int.Parse(customerID));
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "*** SQL ERROR! Unable to [Get] order number... ***");
                return StatusCode(500);
            }
            return orderNum;
        }

        // GET api/order/inventory?productName={name}&locationID={id}
        [HttpGet("inventory")]
        public async Task<ActionResult<int>> GetInventoryAmountAsync([FromQuery, Required] ProductName product)
        {
            int amount;
            try
            {
                _logger.LogInformation("*** [GET] inventory amount for product: {product} ***", product.productName);
                amount = await _repository.InventoryAmountAsync(product.productName!, product.locationID);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "*** SQL ERROR! Unable to [Get] inventory amount... ***");
                return StatusCode(500);
            }
            return amount;
        }

        // GET api/order?customerID={id}&orderNum={num}

        // GET api/order?customerID={id}

        // GET api/order?mostrecent=true

        // GET api/order?customerID={cid}&locationID={lid}
    }
}
