namespace StoreApi.Logic
{
    public class Order
    {
        /*
         *      OrderNum ?
         *      CustomerID, 
         *      List<string> ProductName (data take from productList), 
         *      List<int> ProductQty,
         *      int LocationID
         */
        /// <summary>
        ///     Store Order number from 'OrderProduct' db table or user input
        /// </summary>
        public int OrderNum { get; }
        /// <summary>
        ///     store product name from 'OrderProduct' db table or user input
        /// </summary>
        public string ProductName { get; }
        /// <summary>
        ///     store order product quantity from 'OrderProduct' db table or user input
        /// </summary>
        public int ProductQty { get; }
        /// <summary>
        ///     store location id from 'OrderProduct' db table or user input
        /// </summary>
        public int LocationID { get; }
        /// <summary>
        ///     store order time from 'OrderProduct' db table or user input
        /// </summary>
        public string OrderTime { get; }
        /// <summary>
        ///     Optional field. Use to save store location from 'OrderProduct' db table
        /// </summary>
        public string Location {get;}
        public Order(int orderNum, string productName, int productQty, int locationID, string date = "", string loaction = "")
        {
            OrderNum = orderNum;
            ProductName = productName;
            ProductQty = productQty;
            LocationID = locationID;
            OrderTime = date;
            Location = loaction;
        }
    }
}
