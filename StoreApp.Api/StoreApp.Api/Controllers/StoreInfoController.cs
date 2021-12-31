using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using StoreApi.DataStorage;
using StoreApi.Logic;

namespace StoreApp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoreInfoController : ControllerBase
    {
        private readonly IRepository _repository;
        public StoreInfoController(IRepository repository)
        {
            _repository = repository;
        }

        // GET api/stores
        [HttpGet]
        public async Task<IEnumerable<Location>> GetStoreLocations()
        {
            IEnumerable<Location> stores = await _repository.GetLocationListAsync();

            return stores.ToList();
        }

        // GET
    }
}
