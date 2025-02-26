using API.Configuration;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace API.Services
{
    /// <summary>
    /// Provides data manipulation functions for the blob storage.
    /// </summary>
    public class BlobStorageService
    {
        // Services
        public readonly BlobServiceClient _blobClient;

        // Variables
        public readonly string            _containerName;

        public BlobStorageService(IOptions<BlobStorageOptions> options, BlobServiceClient blobClient)
        {
            _blobClient    = blobClient;
            _containerName = options.Value.ContainerName;
        }

        /// <summary>
        /// Uploads a file to the Blob Storage container and returns the file name.
        /// </summary>
        /// <param name="file">The file to be uploaded.</param>
        /// <returns>
        /// Returning the name of the uploaded file.
        /// </returns>
        public async Task<string> UploadFileAsync(IFormFile file)
        {
            // gets blob container
            var container = _blobClient.GetBlobContainerClient(_containerName);

            // creates container if not exist - no public access
            await container.CreateIfNotExistsAsync(PublicAccessType.None);

            // gets the blob client
            var blobClient = container.GetBlobClient(file.FileName);

            // uploads the file
            using (var stream = file.OpenReadStream())
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

            // returns the file name
            return file.FileName;
        }

        /// <summary>
        /// Generates and returns the URL of a blob file in the storage container.
        /// </summary>
        /// <param name="fileName">The name of the file in the blob storage.</param>
        /// <returns>
        /// The full URL of the blob file.
        /// </returns>
        public string GetBlobUrl(string fileName)
        {
            // gets blob container
            var container  = _blobClient.GetBlobContainerClient(_containerName);

            // gets the blob client
            var blobClient = container.GetBlobClient(fileName);

            // generate SAS token
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName          = fileName,
                Resource          = "b",
                ExpiresOn         = DateTime.UtcNow.AddMinutes(1)
            };

            // sets the permission - read only access
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // generates and return the SAS URI
            return blobClient.GenerateSasUri(sasBuilder).ToString();
        }

        /// <summary>
        /// Retrieves a list of all blob file URLs from the storage container.
        /// </summary>
        /// <returns>
        /// Returning an enumerable collection of blob URLs.
        /// </returns>
        public async Task<IEnumerable<string>> GetAllBlobsAsync()
        {
            // gets blob container
            var container = _blobClient.GetBlobContainerClient(_containerName);

            // holds temp values
            var fileUrls  = new List<string>();

            await foreach (var blob in container.GetBlobsAsync())
            {
                // gets the blob client
                var blobClient = container.GetBlobClient(blob.Name);

                // generate SAS token
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _containerName,
                    BlobName          = blobClient.Name,
                    Resource          = "b",
                    ExpiresOn         = DateTime.UtcNow.AddMinutes(1)
                };

                // sets the permission - read only access
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                // generates and return the SAS URI
                var uri = blobClient.GenerateSasUri(sasBuilder).ToString();
                fileUrls.Add(uri);
            }

            // returns the url list
            return fileUrls;
        }

        /// <summary>
        /// Deletes a file from the Blob Storage container.
        /// </summary>
        /// <param name="fileName">The name of the file to be deleted.</param>
        /// <returns>
        /// Returning <c>true</c> if the file was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> DeleteFileAsync(string fileName)
        {
            // gets blob container
            var container  = _blobClient.GetBlobContainerClient(_containerName);

            // gets the blob client
            var blobClient = container.GetBlobClient(fileName);

            // returns the result
            return await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
        }
    }
}
