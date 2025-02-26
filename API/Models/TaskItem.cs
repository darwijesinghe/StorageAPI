using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;

namespace API.Models
{
    /// <summary>
    /// Represents a model for a task.
    /// </summary>
    public class TaskItem : ITableEntity
    {
        /// <summary>
        /// The unique ID for the record set
        /// </summary>
        public Guid Id                   { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The name of the task
        /// </summary>
        public string TaskName           { get; set; }

        /// <summary>
        /// The task assignee
        /// </summary>
        public string Assignee           { get; set; }

        /// <summary>
        /// The name of the uploading file
        /// </summary>
        public string? FileName          {  get; set; }

        /// <summary>
        /// The URL of the uploaded file
        /// </summary>
        public string? FileUrl           { get; set; }

        /// <summary>
        /// File as Base64String
        /// </summary>
        public string? Base64File        { get; set; }

        private DateTime _deadline;

        /// <summary>
        /// The deadline for the task
        /// </summary>
        public DateTime Deadline
        {
            get
            {
                return _deadline;
            }
            set
            {
                _deadline = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        // ITableEntity implementations --------------

        public string PartitionKey       { get; set; }
        public string RowKey             { get; set; } = Guid.NewGuid().ToString();
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag                 { get; set; }

        public TaskItem()
        {
            PartitionKey = Id.ToString();
        }
    }
}
