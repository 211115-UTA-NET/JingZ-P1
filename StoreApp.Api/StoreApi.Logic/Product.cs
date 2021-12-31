namespace StoreApi.Logic
{
    public class Product
    {
        /// <summary>
        ///     store product name from 'StoreInventory' database table
        /// </summary>
        public string ProductName { get; }
        /// <summary>
        ///     store price from 'StoreInventory' database table
        /// </summary>
        public decimal Price { get; }
        public Product(string productName, decimal price)
        {
            ProductName = productName;
            Price = price;
        }
    }
}
