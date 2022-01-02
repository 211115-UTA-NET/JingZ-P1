namespace StoreConsoleApp.UI
{
    public interface IStoreService
    {
        public Task<string> GetLocationsAsync();
        public Task<(string, bool)> GetStoreProductsAsync(string locationID);
    }
}