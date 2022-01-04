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
    public class CustomerController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(IRepository repository, ILogger<CustomerController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // POST api/customer
        [HttpPost]
        public async Task<ActionResult<int>> CreateAccount(string firstName, string lastName)
        {
            int id = -1;
            try
            {
                _logger.LogInformation("*** [POST] Add customer: {firstName} {lastName} ***", firstName, lastName);
                id = await _repository.AddNewCustomerAsync(firstName, lastName);
            } catch (SqlException ex)
            {
                _logger.LogError(ex, "*** SQL ERROR! Unable to [POST] Add customer... ***");
                return StatusCode(500);
            }
            return id;
        }

        // GET api/customer?customerid=forgot&firstname=...&lastname=...
        // GET api/customer?customerid=...&firstname=""&lastname=""
        [HttpGet("login")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAccount([FromQuery, Required] CustomerLogin customer)
        {
            IEnumerable<Customer> customerInfo;
            try
            {
                _logger.LogInformation("*** [GET] customer ID# {id}: {firstName} {lastName} ***", customer.CustomerID, customer.FirstName, customer.LastName);
                customerInfo = await _repository.FindCustomerAsync(customer.CustomerID + "", customer.FirstName!, customer.LastName!);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "*** SQL ERROR! Unable to [Get] customer information... ***");
                return StatusCode(500);
            }
            return customerInfo.ToList();
        }
    }
}
