using StoreConsoleApp.UI.Dtos;

namespace StoreConsoleApp.UI
{
    public interface IRequestServices
    {
        public Task<HttpResponseMessage> GetResponseForGETAsync(string requestUri);
        public Task<HttpResponseMessage> GetResponseForPOSTAsync(string jsonContent, string requestUri);
    }
}