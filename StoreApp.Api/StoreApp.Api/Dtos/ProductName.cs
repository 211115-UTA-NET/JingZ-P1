using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Dtos
{
    public class ProductName
    {
        [Required]
        public string? productName { get; set; }

        [Required]
        public int locationID { get; set; }
    }
}
