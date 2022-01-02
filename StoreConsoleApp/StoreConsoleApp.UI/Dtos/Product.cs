namespace StoreConsoleApp.UI
{
    public class Product
    {
        /// <summary>
        ///     store product name from 'StoreInventory' database table
        /// </summary>
        public string? ProductName { get; set; }
        /// <summary>
        ///     store price from 'StoreInventory' database table
        /// </summary>
        public decimal Price { get; set; }
    }
}