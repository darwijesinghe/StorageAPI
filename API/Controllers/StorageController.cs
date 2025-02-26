using API.Helpers;
using API.Models;
using API.Services;
using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        // Services
        private readonly TableStorageService _storageService;
        private readonly BlobStorageService  _blobStorageService;
        private readonly QueueStorageService _queueStorageService;

        public StorageController(TableStorageService storageService, BlobStorageService blobStorageService, QueueStorageService queueStorageService)
        {
            _storageService      = storageService;
            _blobStorageService  = blobStorageService;
            _queueStorageService = queueStorageService;
        }

        /// <summary>
        /// Creates a new item to the table storage.
        /// </summary>
        /// <param name="data">The data to be added to the table.</param>
        /// <returns>
        /// A <see cref="OkResult"/> if success; otherwise error message.
        /// </returns>
        [HttpPost("create-record")]
        public async Task<IActionResult> AddTaskAsync([FromBody] TaskItem data)
        {
            try
            {
                // validations
                if (data is null)
                    return BadRequest("Required data is not provided.");

                // gets the response of the operation
                var response = await _storageService.AddTaskAsync(new TaskItem
                {
                    TaskName = data.TaskName,
                    Assignee = data.Assignee,
                    Deadline = data.Deadline,
                    FileName = data.FileName,
                });

                if(response.IsError)
                    return Problem(response.ReasonPhrase);

                // uploads the file
                if (!string.IsNullOrEmpty(data.FileName) && !string.IsNullOrEmpty(data.Base64File))
                {
                    // file conversion
                    var file = Helper.ConvertBase64ToIFromFile(data.Base64File, data.FileName);
                    await _blobStorageService.UploadFileAsync(file);
                }

                // add message to the queue
                await _queueStorageService.SendMessageAsync(new TaskMessage
                {
                    Message = "A new task successfully added."
                });

                // returns the result
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a task by its partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key of the task to be retrieved.</param>
        /// <param name="rowKey">The row key of the task to be retrieved.</param>
        /// <returns>
        /// A <see cref="OkObjectResult"/> if success; otherwise error message.
        /// </returns>
        [HttpGet("get-task")]
        public async Task<IActionResult> GetTaskAsync(string partitionKey, string rowKey, string fileName)
        {
            try
            {
                // validations
                if(string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey) || string.IsNullOrEmpty(fileName))
                    return BadRequest("Required data is not provided.");

                // gets the result of the operation
                var result = await _storageService.GetTaskAsync<TaskItem>(partitionKey, rowKey);
                if (result is null)
                    return BadRequest("Required data not found.");

                // gets the uploaded file URL
                var fileUrl    = _blobStorageService.GetBlobUrl(fileName);
                result.FileUrl = fileUrl;

                // gets the queued message for test purposes
                var message = await _queueStorageService.ReceiveMessageAsync();
                if (message is not null)
                    return Ok(new { data = result, message });

                // returns the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves the all the data.
        /// </summary>
        /// <returns>
        /// A <see cref="OkObjectResult"/> if success; otherwise error message.
        /// </returns>
        [HttpGet("get-tasks")]
        public async Task<IActionResult> GetTasksAsync()
        {
            try
            {
                // gets the result of the operation
                var result = await _storageService.GetTasksAsync<TaskItem>();
                if (result is null || !result.Any())
                    return BadRequest("Required data not found.");

                // sets the file url
                foreach (var task in result)
                    task.FileUrl = _blobStorageService.GetBlobUrl(task.FileName);

                // returns the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        /// <summary>
        /// Deletes an existing record using partition key and row key.
        /// </summary>
        /// <param name="partitionKey">The partition key of the task to be retrieved.</param>
        /// <param name="rowKey">The row key of the task to be retrieved.</param>
        /// <returns>
        /// A <see cref="OkResult"/> if success; otherwise error message.
        /// </returns>
        [HttpDelete("delete-task")]
        public async Task<IActionResult> DeleteItemAsync(string partitionKey, string rowKey, string fileName)
        {
            try
            {
                // validations
                if (string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey) || string.IsNullOrEmpty(fileName))
                    return BadRequest("Required data is not provided.");

                // gets the response of the operation
                var response = await _storageService.DeleteTaskAsync(partitionKey, rowKey);
                if (response.IsError)
                    return Problem(response.ReasonPhrase);

                // deletes the uploaded file
                var result = await _blobStorageService.DeleteFileAsync(fileName);
                if(!result)
                    return Ok("Uploaded file is not successfully deleted.");

                // returns the result
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
