using System.ComponentModel.DataAnnotations;

namespace StoreApp.Api.Dtos
{
    public class OrderInfo
    {
        /// <summary>
        ///     Store Order number from 'OrderProduct' db table or user input
        /// </summary>
        [Required]
        public int OrderNum { get; set; }
        /// <summary>
        ///     store product name from 'OrderProduct' db table or user input
        /// </summary>
        [Required]
        public string? ProductName { get; set; }
        /// <summary>
        ///     store order product quantity from 'OrderProduct' db table or user input
        /// </summary>
        [Required]
        public int ProductQty { get; set; }
        /// <summary>
        ///     store location id from 'OrderProduct' db table or user input
        /// </summary>
        [Required]
        public int LocationID { get; set; }
        /// <summary>
        ///     store order time from 'OrderProduct' db table or user input
        /// </summary>
        [Required]
        public string? OrderTime { get; set; } = "";
        /// <summary>
        ///     Optional field. Use to save store location from 'OrderProduct' db table
        /// </summary>
        [Required]
        public string Location { get; set; } = "";
    }
}
