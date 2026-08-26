using Microsoft.Extensions.Configuration;

namespace Scheduler.API.Common
{
    public interface IUrlService
    {
        string BuildWebPath(string filePath);
        string BuildImageUrl(string imagePath);
        bool IsCompleteUrl(string path);
        string GetStorageBaseUrl();
        string GetStorageType();
    }

    public class UrlService : IUrlService
    {
        private readonly IConfiguration _configuration;

        public UrlService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string BuildWebPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return filePath;

            // If it's already a complete URL, return as is
            if (IsCompleteUrl(filePath))
                return filePath;

            var storageType = GetStorageType();
            var baseUrl = GetStorageBaseUrl();

            switch (storageType?.ToLower())
            {
                case "s3":
                case "aws":
                    return BuildS3Url(filePath, baseUrl);
                case "azure":
                    return BuildAzureUrl(filePath, baseUrl);
                case "gcp":
                case "google":
                    return BuildGcpUrl(filePath, baseUrl);
                case "local":
                default:
                    return BuildLocalUrl(filePath, baseUrl);
            }
        }

        public string BuildImageUrl(string imagePath)
        {
            return BuildWebPath(imagePath);
        }

        public bool IsCompleteUrl(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            return path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                   path.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        public string GetStorageBaseUrl()
        {
            return _configuration["Storage:BaseUrl"] ?? 
                   _configuration["FileStorage:BaseUrl"] ?? 
                   ""; // Empty for direct static file serving
        }

        public string GetStorageType()
        {
            return _configuration["Storage:Type"] ?? 
                   _configuration["FileStorage:Type"] ?? 
                   "local";
        }

        private string BuildLocalUrl(string filePath, string baseUrl)
        {
            // Handle local file storage
            var localBaseDir = _configuration["Storage:LocalBaseDir"] ?? 
                              _configuration["FileStorage:LocalBaseDir"] ?? 
                              @"C:\FileStorage";

            if (filePath.StartsWith(localBaseDir, StringComparison.OrdinalIgnoreCase))
            {
                var relative = filePath.Substring(localBaseDir.Length).Replace("\\", "/");
                
                // If baseUrl is empty, use direct static file serving
                if (string.IsNullOrEmpty(baseUrl))
                {
                    return relative.StartsWith("/") ? relative : "/" + relative;
                }
                else
                {
                    return baseUrl + (relative.StartsWith("/") ? relative : "/" + relative);
                }
            }

            // Check if the path already starts with the base URL to avoid duplication
            if (!string.IsNullOrEmpty(baseUrl) && filePath.StartsWith(baseUrl, StringComparison.OrdinalIgnoreCase))
            {
                return filePath; // Already has the base URL, return as is
            }

            // If it doesn't start with the base dir, assume it's already relative
            return baseUrl + (filePath.StartsWith("/") ? filePath : "/" + filePath);
        }

        private string BuildS3Url(string filePath, string baseUrl)
        {
            // For S3, the filePath should be the S3 key
            // baseUrl should be the S3 bucket URL or CDN URL
            if (string.IsNullOrEmpty(baseUrl))
            {
                var bucketName = _configuration["Storage:S3:BucketName"];
                var region = _configuration["Storage:S3:Region"];
                return $"https://{bucketName}.s3.{region}.amazonaws.com/{filePath}";
            }

            return baseUrl.TrimEnd('/') + "/" + filePath.TrimStart('/');
        }

        private string BuildAzureUrl(string filePath, string baseUrl)
        {
            // For Azure Blob Storage
            if (string.IsNullOrEmpty(baseUrl))
            {
                var accountName = _configuration["Storage:Azure:AccountName"];
                var containerName = _configuration["Storage:Azure:ContainerName"];
                return $"https://{accountName}.blob.core.windows.net/{containerName}/{filePath}";
            }

            return baseUrl.TrimEnd('/') + "/" + filePath.TrimStart('/');
        }

        private string BuildGcpUrl(string filePath, string baseUrl)
        {
            // For Google Cloud Storage
            if (string.IsNullOrEmpty(baseUrl))
            {
                var bucketName = _configuration["Storage:GCP:BucketName"];
                return $"https://storage.googleapis.com/{bucketName}/{filePath}";
            }

            return baseUrl.TrimEnd('/') + "/" + filePath.TrimStart('/');
        }
    }
} 