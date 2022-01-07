using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Controllers
{
    public class SpecificOrders
    {
        [Required]
        public int customerId { get; set; }
        [Required]
        public int orderNum { get; set; }
    }
}