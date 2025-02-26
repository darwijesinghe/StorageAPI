using API.Models;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Newtonsoft.Json;

namespace API.Services
{
    /// <summary>
    /// Provides data manipulation functions for the queue storage.
    /// </summary>
    public class QueueStorageService
    {
        // Services
        private readonly QueueClient _queueClient;

        public QueueStorageService(QueueClient queueClient)
        {
            _queueClient = queueClient;
            _queueClient.CreateIfNotExists();
        }

        /// <summary>
        /// Sends a message to an Azure Queue Storage.
        /// </summary>
        /// <param name="message">The message to be added to the queue.</param>
        public async Task SendMessageAsync(TaskMessage message)
        {
            // converts to json object
            var serializedMessage = JsonConvert.SerializeObject(message);

            // sends to the queue
            if (_queueClient.Exists())
                await _queueClient.SendMessageAsync(serializedMessage);
        }

        /// <summary>
        /// Retrieves message from Azure Queue Storage.
        /// </summary>
        /// <returns>
        /// The type of <see cref="TaskMessage"/> if a message is available; otherwise null.
        /// </returns>
        public async Task<TaskMessage> ReceiveMessageAsync()
        {
            if (_queueClient.Exists())
            {
                // gets the messages
                QueueMessage[] messages = await _queueClient.ReceiveMessagesAsync(1);
                if (messages.Length > 0)
                {
                    // gets the message
                    var message             = messages[0].Body.ToString();

                    // converts back to the class object
                    var deserializedMessage = JsonConvert.DeserializeObject<TaskMessage>(message);

                    // delete the message from the queue after reading
                    await _queueClient.DeleteMessageAsync(messages[0].MessageId, messages[0].PopReceipt);

                    // returns the result
                    return deserializedMessage;
                }
            }

            return null;
        }
    }
}
