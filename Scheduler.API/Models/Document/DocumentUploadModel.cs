namespace Scheduler.API.Models.Document
{
    public class DocumentUploadModel
    {
        public IFormFile DocumentData { get; set; }
        public int DocumentTypeId { get; set; }
        public Guid UserId { get; set; }
        public string? AccessRoles { get; set; }
        public string? DocumentPath { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
