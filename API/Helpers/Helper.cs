namespace API.Helpers
{
    /// <summary>
    /// Provides the helper methods.
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Converts Base64String to the IFromFile.
        /// </summary>
        /// <param name="base64String">The converted string to the base64.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>
        /// The type of <see cref="IFormFile"/> representing the file from the provided Base64 string.
        /// </returns>
        public static IFormFile ConvertBase64ToIFromFile(string base64String, string fileName)
        {
            try
            {
                // removes the "data:image/png;base64," or any other data URL scheme prefix if present
                var base64Data   = base64String.Contains(",") ? base64String.Split(',')[1] : base64String;

                // converts Base64 string to byte array
                byte[] fileBytes = Convert.FromBase64String(base64Data);

                // creates a memory stream from the byte array
                var stream       = new MemoryStream(fileBytes);

                // creates an IFormFile from the memory stream
                var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
                {
                    Headers     = new HeaderDictionary(),
                    ContentType = "application/octet-stream"
                };

                // returns the file
                return formFile;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
