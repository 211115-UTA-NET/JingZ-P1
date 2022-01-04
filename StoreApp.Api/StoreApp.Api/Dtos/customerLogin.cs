using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Dtos
{
    public class CustomerLogin
    {
        /// <summary>
        ///     store customer ID from 'Customer' db table or user input
        /// </summary>
        [Required]
        public string? CustomerID { get; set; }
        /// <summary>
        ///     store customer last name from 'Customer' db table or user input
        /// </summary>
        [Required]
        public string? LastName { get; set; }
        /// <summary>
        ///     store customer first name from 'Customer' db table or user input
        /// </summary>
        [Required]
        public string? FirstName { get; set; }
    }
}
