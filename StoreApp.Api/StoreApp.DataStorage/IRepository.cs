using StoreApi.Logic;

namespace StoreApi.DataStorage
{
    public interface IRepository
    {
        Task<IEnumerable<Location>> GetLocationListAsync();
        Task<IEnumerable<Product>> GetStoreProductsAsync(string locationID);
        Task<int> AddNewCustomerAsync(string firstName, string lastName);
        Task<int> InventoryAmountAsync(string productName, int locationID);
        Task<int> GetOrderNumberAsync(int customerID);
        Task<IEnumerable<Order>> AddOrderAsync(List<Order> order);
        Task<List<decimal>> GetPriceAsync(List<Order> order);
        Task<IEnumerable<Customer>> FindCustomerAsync(string customerID, string firstName, string lastName);
        Task<IEnumerable<Order>> GetLocationOrdersAsync(int customerID, int locationID);
        Task<IEnumerable<Order>> GetStoreOrdersAsync(int customerID);
        Task<IEnumerable<Order>> GetMostRecentOrderAsync(int customerID);
        Task<IEnumerable<Order>> GetSpecificOrderAsync(int customerID, int orderNum);
    }
}