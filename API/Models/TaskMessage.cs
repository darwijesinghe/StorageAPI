namespace API.Models
{
    /// <summary>
    /// Represents a model for a queue message.
    /// </summary>
    public class TaskMessage
    {
        /// <summary>
        /// The message is to be sent after adding the new task
        /// </summary>
        public string Message { get; set; }
    }
}
