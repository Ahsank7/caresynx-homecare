using Scheduler.API.Helper;

namespace Scheduler.API.Models.Document
{
    public class DocumentSearchRequest
    {
        public Guid? UserId { get; set; }
        public DocumentType? DocumentTypeId { get; set; }
        public string? SortColumn { get; set; }
        public string? SortType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
