namespace API.Configuration
{
    /// <summary>
    /// Provides configuration options.
    /// </summary>
    public class BlobStorageOptions : StorageBaseOptions
    {
        /// <summary>
        /// The name of the storage container
        /// </summary>
        public string ContainerName { get; set; }
    }
}
