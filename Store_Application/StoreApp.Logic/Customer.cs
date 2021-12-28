namespace StoreApp.Logic
{
    public class Customer
    {
        /// <summary>
        ///     store customer ID from 'Customer' db table or user input
        /// </summary>
        public int CustomerID { get; }
        /// <summary>
        ///     store customer last name from 'Customer' db table or user input
        /// </summary>
        public string LastName { get; }
        /// <summary>
        ///     store customer first name from 'Customer' db table or user input
        /// </summary>
        public string FirstName { get; }
        public Customer(int customerID, string firstName, string lastName)
        {
            CustomerID = customerID;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
