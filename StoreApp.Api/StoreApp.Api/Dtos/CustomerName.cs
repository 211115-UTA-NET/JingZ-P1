using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Dtos
{
    public class CustomerName
    {
        [Required]
        public string? firstName { get; set; }

        [Required]
        public string? lastName { get; set; }

    }
}
