using StoreApi.Logic;

namespace StoreApi.DataStorage
{
    public interface IRepository
    {
        IEnumerable<Location> GetLocationList();
        IEnumerable<Product> GetStoreProducts(string locationID);
        int AddNewCustomer(string firstName, string lastName);
        int InventoryAmount(string productName, int locationID);
        int GetOrderNumber(int customerID);
        IEnumerable<Order> AddOrder(List<Order> order);
        List<decimal> GetPrice(List<Order> order);
        IEnumerable<Customer> FindCustomer(string customerID, string firstName, string lastName);
        IEnumerable<Order> GetLocationOrders(int customerID, int locationID);
        IEnumerable<Order> GetStoreOrders(int customerID);
        IEnumerable<Order> GetMostRecentOrder(int customerID);
        IEnumerable<Order> GetSpecificOrder(int customerID, int orderNum);
    }
}