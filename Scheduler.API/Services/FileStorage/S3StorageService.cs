using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Scheduler.API.Services.FileStorage
{
    public class S3StorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _region;

        public S3StorageService(IConfiguration configuration)
        {
            var accessKey = configuration["Storage:S3:AccessKey"];
            var secretKey = configuration["Storage:S3:SecretKey"];
            _region = configuration["Storage:S3:Region"] ?? "us-east-1";
            _bucketName = configuration["Storage:S3:BucketName"] ?? "your-s3-bucket";

            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("AWS S3 access key and secret key must be configured.");
            }

            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(_region)
            };

            _s3Client = new AmazonS3Client(accessKey, secretKey, config);
        }

        public async Task<string> SaveFileAsync(IFormFile file, string directoryPath)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var key = $"{directoryPath}/{fileName}";

            using var stream = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.AES256
            };

            await _s3Client.PutObjectAsync(request);

            // Return the S3 URL
            return $"https://{_bucketName}.s3.{_region}.amazonaws.com/{key}";
        }

        public async Task DeleteFileAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            try
            {
                var uri = new Uri(filePath);
                var key = uri.AbsolutePath.TrimStart('/');

                var request = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                await _s3Client.DeleteObjectAsync(request);
            }
            catch (Exception)
            {
                // Log error but don't throw to avoid breaking the application
                // In production, you might want to log this properly
            }
        }
    }
}
