using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Scheduler.API.Services.FileStorage
{
    public interface IFileStorageFactory
    {
        IFileStorageService CreateStorageService();
    }

    public class FileStorageFactory : IFileStorageFactory
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public FileStorageFactory(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public IFileStorageService CreateStorageService()
        {
            var storageType = _configuration["Storage:Type"]?.ToLower() ?? "local";

            return storageType switch
            {
                "azure" => new AzureBlobStorageService(_configuration),
                "s3" or "aws" => new S3StorageService(_configuration),
                "local" or _ => new LocalFileStorageService(_configuration, _environment)
            };
        }
    }
}
