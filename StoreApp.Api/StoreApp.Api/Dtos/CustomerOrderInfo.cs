using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Controllers
{
    public class CustomerOrderInfo
    {
        [Required]
        public int customerId { get; set; }
        public int orderNum { get; set; } = -1; //optional
        public int locationId { get; set; } = -1; //optional
    }
}