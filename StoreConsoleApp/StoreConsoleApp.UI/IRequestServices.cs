namespace StoreConsoleApp.UI
{
    public interface IRequestServices
    {
        public Task<HttpResponseMessage> GetResponseMessageAsync(string requestUri);
    }
}