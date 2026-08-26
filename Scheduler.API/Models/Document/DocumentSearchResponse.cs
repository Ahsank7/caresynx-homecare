using Scheduler.API.Models.Contact;

namespace Scheduler.API.Models.Document
{
    public class DocumentSearchResponse
    {
        public List<DocumentInfo>? Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
