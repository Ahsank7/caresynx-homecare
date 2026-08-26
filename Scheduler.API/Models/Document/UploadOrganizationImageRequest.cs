namespace Scheduler.API.Models.Document
{
    public class UploadOrganizationImageRequest
    {
        public IFormFile File { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
