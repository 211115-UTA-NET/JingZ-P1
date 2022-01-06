using StoreConsoleApp.UI.Dtos;

namespace StoreConsoleApp.UI
{
    public interface IRequestServices
    {
        public Task<HttpResponseMessage> GetResponseAsync(string requestUri);
        public Task<HttpResponseMessage> GetResponseForAddCustomerAsync(CustomerName customerName);
        public Task<HttpResponseMessage> GetResponseForPlaceOrderAsync(List<Order> orders);
    }
}