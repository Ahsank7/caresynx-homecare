using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace Scheduler.API.Services.FileStorage
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly string _baseDirectory;

        public LocalFileStorageService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            // Use the new Storage configuration instead of FileStorage
            var basePath = configuration["Storage:LocalBaseDir"] ?? "wwwroot/FileStorage";
            
            // Extract the actual path from the URL if it's a full URL
            if (basePath.StartsWith("http"))
            {
                // Extract path from URL like "https://localhost:7094/FileStorage"
                var uri = new Uri(basePath);
                basePath = uri.AbsolutePath.TrimStart('/');
            }
            
            // Ensure we have an absolute path
            if (!Path.IsPathRooted(basePath))
            {
                _baseDirectory = Path.Combine(environment.ContentRootPath, basePath);
            }
            else
            {
                _baseDirectory = basePath;
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string directoryPath)
        {
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var fullPath = Path.Combine(_baseDirectory, directoryPath, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fullPath;
        }

        public Task DeleteFileAsync(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }
    }
}
