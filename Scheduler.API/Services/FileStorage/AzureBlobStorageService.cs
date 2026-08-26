using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Scheduler.API.Services.FileStorage
{
    public class AzureBlobStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public AzureBlobStorageService(IConfiguration configuration)
        {
            var connectionString = configuration["Storage:Azure:ConnectionString"] ?? 
                                 configuration["AzureStorage:ConnectionString"];
            var accountName = configuration["Storage:Azure:AccountName"];
            var accountKey = configuration["Storage:Azure:AccountKey"];
            
            if (string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(accountKey))
            {
                connectionString = $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey};EndpointSuffix=core.windows.net";
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Azure Storage connection string or account credentials must be configured.");
            }

            _blobServiceClient = new BlobServiceClient(connectionString);
            _containerName = configuration["Storage:Azure:ContainerName"] ?? "documents";
        }

        public async Task<string> SaveFileAsync(IFormFile file, string directoryPath)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobName = $"{directoryPath}/{fileName}";

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

            var blobClient = containerClient.GetBlobClient(blobName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            // Return the blob URL
            return blobClient.Uri.ToString();
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            try
            {
                var uri = new Uri(filePath);
                var containerName = uri.Segments[1].TrimEnd('/');
                var blobName = string.Join("/", uri.Segments.Skip(2));

                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception)
            {
                // Log error but don't throw to avoid breaking the application
                // In production, you might want to log this properly
            }
        }
    }
}
