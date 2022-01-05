namespace StoreConsoleApp.UI
{
    public interface IRequestServices
    {
        public Task<HttpResponseMessage> GetResponseAsync(string requestUri);
    }
}