namespace API.Configuration
{
    /// <summary>
    /// Provides configuration options.
    /// </summary>
    public class TableStorageOptions : StorageBaseOptions
    {
        /// <summary>
        /// The name of the table
        /// </summary>
        public string TableName { get; set; }
    }
}
