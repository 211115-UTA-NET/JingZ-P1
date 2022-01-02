namespace StoreConsoleApp.UI.Dtos
{
    public class Location
    {
        /// <summary>
        ///     Store location id from 'Location' db table
        /// </summary>
        public int LocationID { get; set; }
        /// <summary>
        ///     save store location id from 'Location' db table
        /// </summary>
        public string? StoreLocation { get; set; }
    }
}