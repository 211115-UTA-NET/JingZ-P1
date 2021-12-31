namespace StoreApi.Logic
{
    public class Location
    {
        /// <summary>
        ///     Store location id from 'Location' db table
        /// </summary>
        public int LocationID { get; }
        /// <summary>
        ///     save store location id from 'Location' db table
        /// </summary>
        public string StoreLocation { get; }
        public Location(int id, string location)
        {
            LocationID = id;
            StoreLocation = location;
        }
    }
}