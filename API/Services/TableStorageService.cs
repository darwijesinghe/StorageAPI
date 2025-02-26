using Azure.Data.Tables;

namespace API.Services
{
    /// <summary>
    /// Provides data manipulation functions for the table storage.
    /// </summary>
    public class TableStorageService
    {
        // Services
        private readonly TableClient _tableClient;

        public TableStorageService(TableClient tableClient)
        {
            _tableClient = tableClient;

            // creates the table if it already not exist
            _tableClient.CreateIfNotExists();
        }

        /// <summary>
        /// Asynchronously adds or updates an entity in Azure Table Storage.
        /// </summary>
        /// <param name="data">The entity to be added or updated.</param>
        /// <returns>
        /// The response from Azure Table Storage.
        /// </returns>
        public async Task<Azure.Response> AddTaskAsync<T>(T data) where T : class, ITableEntity
        {
            // returns the result
            return await _tableClient.UpsertEntityAsync(data, TableUpdateMode.Replace);
        }

        /// <summary>
        /// Asynchronously retrieves an entity from Azure Table Storage using the specified partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key of the entity.</param>
        /// <param name="rowKey">The row key of the entity.</param>
        /// <returns>
        /// The result of type <typeparamref name="T"/>.
        /// </returns>
        public async Task<T> GetTaskAsync<T>(string partitionKey, string rowKey) where T : class, ITableEntity
        {
            // returns the result
            return await _tableClient.GetEntityAsync<T>(partitionKey, rowKey);
        }

        /// <summary>
        /// Asynchronously retrieves all tasks of a specified type from Azure Table Storage.
        /// </summary>
        /// <returns>
        /// A collection of type <see cref="IEnumerable{T}"/>.
        /// </returns>
        public async Task<IEnumerable<T>> GetTasksAsync<T>() where T : class, ITableEntity
        {
            // temp variable
            var entities = new List<T>();

            // adds the each entity to the collection
            await foreach (var entity in _tableClient.QueryAsync<T>())
                entities.Add(entity);

            // returns the result
            return entities;
        }

        /// <summary>
        /// Asynchronously deletes an entity from Azure Table Storage using the specified partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key of the entity.</param>
        /// <param name="rowKey">The row key of the entity.</param>
        /// <returns>
        /// The response from Azure Table Storage.
        /// </returns>
        public async Task<Azure.Response> DeleteTaskAsync(string partitionKey, string rowKey)
        {
            // returns the result
            return await _tableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
    }
}
