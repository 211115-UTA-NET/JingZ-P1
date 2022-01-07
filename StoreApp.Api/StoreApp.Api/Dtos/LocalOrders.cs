using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Controllers
{
    public class LocalOrders
    {
        [Required]
        public int customerId { get; set; }
        [Required]
        public int locationId { get; set; }
    }
}