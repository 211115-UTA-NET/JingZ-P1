namespace StoreConsoleApp.UI.Dtos
{
    public class Customer
    {
        /// <summary>
        ///     store customer ID from 'Customer' db table or user input
        /// </summary>
        public int CustomerID { get; set; }
        /// <summary>
        ///     store customer last name from 'Customer' db table or user input
        /// </summary>
        public string? LastName { get; set; }
        /// <summary>
        ///     store customer first name from 'Customer' db table or user input
        /// </summary>
        public string? FirstName { get; set; }

    }
}
