using StoreApi.Logic;
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreApi.DataStorage
{
    public class SqlRepository : IRepository
    {
        private readonly string _connectionString;
        /// <summary>
        ///     initialize connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SqlRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        /// <summary>
        ///     A method takes a query parameters and returns a DataSet type result. 
        ///     Only for read only database connection. (ex. SELECT)
        ///     Using Disconnected Architecture: closeing the connection ASAP
        ///     Notes: paramName and inputVal parameters are optional.
        /// </summary>
        /// <param name="query">Database query command.</param>
        /// <param name="paramName">parameter name in your query (ex. "...WHERE ID = @id" paramName = "id")</param>
        /// <param name="inputVal">the value for the query search condition</param>
        /// <returns>DataSet type</returns>
        private DataSet DBReadOnly(string? query, string[]? paramName = null, object[]? inputVal = null)
        {
            // open connection
            using SqlConnection connection = new(_connectionString);
            connection.Open();
            using SqlCommand command = new(query, connection);
            // prevent sql injection
            if (paramName != null && inputVal != null)
            {
                for (int i = 0; i < inputVal.Length; i++)
                {
                    command.Parameters.AddWithValue($"@{paramName[i]}", inputVal[i]);
                }
            }
            // use data adapter to fill a dataset with the command
            using SqlDataAdapter adapter = new(command);
            DataSet dataSet = new();
            // execute the command
            adapter.Fill(dataSet);
            // close the connection
            connection.Close();
            return dataSet;
        }

        /// <summary>
        ///     A method takes a query parameters and returns number of row affected by the insertion. 
        ///     Only for write only database connection. (ex. INSERT)
        ///     Notes: paramName and inputVal parameters are optional.
        /// </summary>
        /// <param name="query">Database query command.</param>
        /// <param name="paramName">parameter name in your query (ex. "...WHERE ID = @id" paramName = "id")</param>
        /// <param name="inputVal">the value for the query search condition</param>
        /// <returns>Number of rows affected by the insertion</returns>
        private int DBWriteOnly(string? query, string[]? paramName = null, object[]? inputVal = null)
        {
            // open connection
            using SqlConnection connection = new(_connectionString);
            connection.Open();
            using SqlCommand command = new(query, connection);
            // prevent sql injection
            if (paramName != null && inputVal != null)
            {
                for (int i = 0; i < inputVal.Length; i++)
                {
                    command.Parameters.AddWithValue($"@{paramName[i]}", inputVal[i]);
                }
            }
            // execute the command
            int rowsAffected = command.ExecuteNonQuery();
            // close the connection
            connection.Close();
            return rowsAffected;
        }

        /// <summary>
        ///     Get location List from database
        /// </summary>
        /// <returns>A Location class type collection</returns>
        public IEnumerable<Location> GetLocationList()
        {
            List<Location> result = new();
            DataSet dataSet = DBReadOnly("SELECT * FROM Location");
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                result.Add(new((int)row["ID"], (string)row["StoreLocation"]));
            }
            return result;
        }

        /// <summary>
        ///     Get sales products and price from store location
        /// </summary>
        /// <param name="locationID">location id</param>
        /// <returns>A Product class type collection</returns>
        public IEnumerable<Product> GetStoreProducts(string locationID)
        {
            List<Product> result = new();
            bool isInt = int.TryParse(locationID, out int locId);
            if (isInt)
            {
                DataSet dataSet = DBReadOnly("SELECT ProductName, Price FROM StoreInventory WHERE LocationID = @locID ORDER BY Price",
                    new string[] { "locID" },
                    new object[] { locId });
                foreach(DataRow row in dataSet.Tables[0].Rows)
                {
                    result.Add(new((string)row["ProductName"], (decimal)row["Price"]));
                }

            }
            // if locationID is not int then return empty List
            return result;
        }

        /// <summary>
        ///     Insert new customer name to the database and return the customer id.
        /// </summary>
        /// <param name="firstName">customer first name</param>
        /// <param name="lastName">customer last name</param>
        /// <returns>A cutomer id, if return -1 means failed the customer insertion.</returns>
        public int AddNewCustomer(string firstName, string lastName)
        {
            int result = DBWriteOnly("INSERT Customer VALUES (@firstName, @lastName);",
                new string[] { "firstName", "lastName" },
                new object[] { firstName, lastName });
            if (result > 0)
            {
                DataSet customerID = DBReadOnly("SELECT ID FROM Customer WHERE FirstName=@firstName AND LastName=@lastName",
                    new string[] { "firstName", "lastName" },
                    new object[] { firstName, lastName });
                return (int)customerID.Tables[0].Rows[0]["ID"];
            }
            else return -1;
        }

        /// <summary>
        ///     If customerID = 'forgot', then search by customer name.
        ///     else search customer by customer id
        /// </summary>
        /// <param name="customerID">customerID</param>
        /// <param name="firstName">optional</param>
        /// <param name="lastName">optional</param>
        /// <returns>Customer informations</returns>
        public IEnumerable<Customer> FindCustomer(string customerID, string firstName = "", string lastName = "")
        {
            List<Customer> customer = new();
            bool isInt = int.TryParse(customerID, out int CustomerID);
            if (isInt) {
                DataSet dataSet = DBReadOnly("SELECT * FROM Customer WHERE ID = @CustomerID;",
                    new string[] { "CustomerID" },
                    new object[] { CustomerID });
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    customer.Add(new((int)row["ID"], (string)row["FirstName"], (string)row["LastName"]));
                }
            } else if(!isInt && customerID.ToLower() == "forgot")
            {
                DataSet dataSet = DBReadOnly("SELECT * FROM Customer WHERE FirstName=@firstName AND LastName=@lastName;",
                    new string[] { "firstName", "lastName" },
                    new object[] { firstName, lastName });
                foreach (DataRow row in dataSet.Tables[0].Rows)
                {
                    customer.Add(new((int)row["ID"], (string)row["FirstName"], (string)row["LastName"]));
                }
            }
            return customer;
        }

        /// <summary>
        ///     Find theinventory product amount based on the location id and product name.
        /// </summary>
        /// <param name="productName">Valid product name</param>
        /// <param name="locationID">Valid location ID</param>
        /// <returns>The product amount in the local inventory.</returns>
        public int InventoryAmount(string productName, int locationID)
        {
            DataSet dataSet = DBReadOnly(
                "SELECT ProductAmount From StoreInventory WHERE LocationID = @locationID AND ProductName = @productName;",
                new string[] { "locationID", "productName" },
                new object[] { locationID, productName });
            int amount = (int)dataSet.Tables[0].Rows[0]["ProductAmount"];
            return amount;
        }

        /// <summary>
        ///     Insert customer order to CustomerOrder db table, 
        ///     then return the order number of the order.
        /// </summary>
        /// <param name="customerID">customer id</param>
        /// <returns>Order number of the new order</returns>
        public int GetOrderNumber(int customerID)
        {
            int result = DBWriteOnly("INSERT CustomerOrder VALUES (@customerID)",
                new string[] { "customerID" },
                new object[] { customerID });
            if (result > 0)
            {
                DataSet dataSet = DBReadOnly("SELECT MAX(OrderNum) AS OrderNum From CustomerOrder WHERE CustomerID = @customerID;",
                    new string[] { "customerID" },
                    new object[] { customerID });
                return (int)dataSet.Tables[0].Rows[0]["OrderNum"];
            }
            return -1;
        }
        /// <summary>
        ///     Iteratly insert the Order List data to the database, and update inventory amount.
        ///     Return summary of the insertion as receipt.
        /// </summary>
        /// <param name="order">Order type class</param>
        /// <returns>Summary of the insertion as receipt.</returns>
        public IEnumerable<Order> AddOrder(List<Order> order)
        {
            List<Order> receipt = new();
            foreach (Order orderProduct in order)
            {
                // insert order product & update inventory amount
                if (!InsertOrderProduct(orderProduct) || !UpdateInventoryAmount(orderProduct))
                    break;
            }
            // all success return receipt
            DataSet dataSet = DBReadOnly("SELECT OrderNum, ProductName, Amount, LocationID, OrderTime, StoreLocation " +
                "FROM OrderProduct, Location " +
                "WHERE LocationID = Location.ID AND OrderNum = @orderNum;",
                    new string[] { "orderNum" },
                    new object[] { order[0].OrderNum });
            return AddOrderHistoryDatatoList(dataSet);
        }

        /// <summary>
        ///     Insert a single order product to the database. 
        /// </summary>
        /// <param name="order">Order class type</param>
        /// <returns>true if insert success, false otherwise.</returns>
        private bool InsertOrderProduct(Order order)
        {
            int result = DBWriteOnly("INSERT OrderProduct (OrderNum, ProductName, Amount, LocationID) " +
                "VALUES (@orderNum, @productName, @amount, @locationID);",
                new string[] { "orderNum", "productName", "amount", "locationID" },
                new object[] { order.OrderNum, order.ProductName, order.ProductQty, order.LocationID });
            if (result > 0) return true;
            return false;
        }

        /// <summary>
        ///     Update inventory table column:
        ///     Subtract the inventory amount by the customer order amount.
        /// </summary>
        /// <param name="order">Order class type</param>
        /// <returns>true if update success, false otherwise.</returns>
        private bool UpdateInventoryAmount(Order order)
        {
            string query = "UPDATE StoreInventory " +
                "SET ProductAmount = ProductAmount - @orderAmount " +
                "WHERE LocationID = @locationID AND ProductName = @productName;";

            int result = DBWriteOnly(query,
                new string[] { "orderAmount", "locationID", "productName" },
                new object[] { order.ProductQty, order.LocationID, order.ProductName });
            if (result > 0) return true;
            return false;
        }

        /// <summary>
        ///     Get product price from the database
        /// </summary>
        /// <param name="order">Order class type</param>
        /// <returns>The product price</returns>
        public List<decimal> GetPrice(List<Order> order)
        {
            List<decimal> price = new();
            foreach (Order item in order)
            {
                DataSet dataSet = DBReadOnly(
                    "SELECT Price FROM StoreInventory WHERE LocationID = @locationID AND ProductName=@productName",
                    new string[] { "locationID", "productName" },
                    new object[] { item.LocationID, item.ProductName });
                price.Add((decimal)dataSet.Tables[0].Rows[0]["Price"]);
            }
            return price;
        }

        private IEnumerable<Order> AddOrderHistoryDatatoList(DataSet dataSet)
        {
            List<Order> orderHistroy = new();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                orderHistroy.Add(new((int)row["OrderNum"], (string)row["ProductName"], (int)row["Amount"],
                    (int)row["LocationID"], row["OrderTime"].ToString(), (string)row["StoreLocation"]));
            }
            return orderHistroy;
        }

        /// <summary>
        ///     Get order history based on the store location of a customer
        /// </summary>
        /// <param name="customerID"></param>
        /// <param name="locationID"></param>
        /// <returns>A Order class type collection that contains the order history</returns>
        public IEnumerable<Order> GetLocationOrders(int customerID, int locationID)
        {
            string query = "SELECT OrderProduct.OrderNum, ProductName, Amount, LocationID, Location.StoreLocation, OrderTime " +
                "FROM CustomerOrder " +
                "INNER JOIN OrderProduct ON CustomerOrder.OrderNum = OrderProduct.OrderNum " +
                "INNER JOIN Location ON LocationID = Location.ID " +
                "WHERE CustomerID = @customerId AND LocationID = @locationId ORDER BY OrderProduct.OrderNum;";
            DataSet dataSet = DBReadOnly(query,
                 new string[] { "customerId", "locationId" },
                 new object[] { customerID, locationID });
            return AddOrderHistoryDatatoList(dataSet);
        }

        /// <summary>
        ///     Get all order history of the customer.
        /// </summary>
        /// <param name="customerID"></param>
        /// <returns>A Order class type collection that contains the order history</returns>
        public IEnumerable<Order> GetStoreOrders(int customerID)
        {
            string query = "SELECT OrderProduct.OrderNum, ProductName, Amount, LocationID, Location.StoreLocation, OrderTime " +
                "FROM CustomerOrder " +
                "INNER JOIN OrderProduct ON CustomerOrder.OrderNum = OrderProduct.OrderNum " +
                "INNER JOIN Location ON LocationID = Location.ID " +
                "WHERE CustomerID = @customerId ORDER BY OrderProduct.OrderNum;";
            DataSet dataSet = DBReadOnly(query,
                 new string[] { "customerId" },
                 new object[] { customerID });
            return AddOrderHistoryDatatoList(dataSet);
        }

        public IEnumerable<Order> GetMostRecentOrder(int customerID)
        {
            string query = "SELECT OrderProduct.OrderNum, ProductName, Amount, LocationID, Location.StoreLocation, OrderTime FROM CustomerOrder " +
                "INNER JOIN OrderProduct ON CustomerOrder.OrderNum = OrderProduct.OrderNum " +
                "INNER JOIN Location ON LocationID = Location.ID " +
                "WHERE CustomerID = @customerID AND CustomerOrder.OrderNum " +
                "= (SELECT MAX(OrderNum) AS OrderNum From CustomerOrder WHERE CustomerID = @customerID2);";
            DataSet dataSet = DBReadOnly(query,
                 new string[] { "customerID", "customerID2" },
                 new object[] { customerID, customerID });
            return AddOrderHistoryDatatoList(dataSet);
        }

        public IEnumerable<Order> GetSpecificOrder(int customerID, int orderNum)
        {
            string query = "SELECT OrderProduct.OrderNum, ProductName, Amount, LocationID, Location.StoreLocation, OrderTime " +
                "FROM CustomerOrder " +
                "INNER JOIN OrderProduct ON CustomerOrder.OrderNum = OrderProduct.OrderNum " +
                "INNER JOIN Location ON LocationID = Location.ID " +
                "WHERE CustomerID = @customerId AND CustomerOrder.OrderNum = @orderNum;";
            DataSet dataSet = DBReadOnly(query,
                 new string[] { "customerId", "orderNum" },
                 new object[] { customerID, orderNum });
            return AddOrderHistoryDatatoList(dataSet);
        }
    }
}
