namespace Scheduler.API.Services.FileStorage
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(IFormFile file, string directoryPath);
        Task DeleteFileAsync(string filePath);
    }
}
