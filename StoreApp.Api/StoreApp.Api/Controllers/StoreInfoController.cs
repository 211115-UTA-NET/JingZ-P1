using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StoreApi.DataStorage;
using StoreApi.Logic;
using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreInfoController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger<StoreInfoController> _logger;
        public StoreInfoController(IRepository repository, ILogger<StoreInfoController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        // GET api/storeinfo
        //[HttpGet]
        //public async Task<IEnumerable<Location>> GetStoreLocations()
        //{
        //    IEnumerable<Location> stores;
        //    stores = await _repository.GetLocationListAsync();
        //    _logger.LogInformation("*** GET location list ***");
        //    return stores.ToList();
        //}

        // ActionResult types represent various HTTP status codes.
        // GET api/storeinfo/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetStoreProducts([Required] string id)
        {
            IEnumerable<Product> products;
            try
            {
                _logger.LogInformation("*** GET products from store id {id} ***", id);
                products = await _repository.GetStoreProductsAsync(id);
            }
            catch (SqlException e)
            {
                _logger.LogError(e, "*** SQL ERROR! Unable to GET store products... ***");
                return StatusCode(500);
            }
            return products.ToList();
        }
    }
}
