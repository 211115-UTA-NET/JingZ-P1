using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Dtos
{
    public class OrderList
    {
        [Required]
        public List<OrderInfo>? orderlist { get; set; }
    }
}
