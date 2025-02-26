namespace API.Configuration
{
    /// <summary>
    /// Provides configuration options.
    /// </summary>
    public class QueueStorageOptions : StorageBaseOptions
    {
        /// <summary>
        /// The name of the queue
        /// </summary>
        public string QueueName { get; set; }
    }
}
