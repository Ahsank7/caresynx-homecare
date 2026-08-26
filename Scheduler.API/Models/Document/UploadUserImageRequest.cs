namespace Scheduler.API.Models.Document
{
    public class UploadUserImageRequest
    {
        public IFormFile File { get; set; }
        public Guid UserId { get; set; }
    }
}
