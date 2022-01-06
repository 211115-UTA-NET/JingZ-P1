namespace StoreConsoleApp.UI.Dtos
{
    public class Order
    {
        /// <summary>
        ///     Store Order number from 'OrderProduct' db table or user input
        /// </summary>
        public int OrderNum { get; set; }
        /// <summary>
        ///     store product name from 'OrderProduct' db table or user input
        /// </summary>
        public string? ProductName { get; set; }
        /// <summary>
        ///     store order product quantity from 'OrderProduct' db table or user input
        /// </summary>
        public int ProductQty { get; set; }
        /// <summary>
        ///     store location id from 'OrderProduct' db table or user input
        /// </summary>
        public int LocationID { get; set; }
        /// <summary>
        ///     store order time from 'OrderProduct' db table or user input
        /// </summary>
        public string OrderTime { get; set; } = "";
        /// <summary>
        ///     Optional field. Use to save store location from 'OrderProduct' db table
        /// </summary>
        public string Location { get; set; } = "";
    }
}
