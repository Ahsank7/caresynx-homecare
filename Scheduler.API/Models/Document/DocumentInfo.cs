using Scheduler.API.Helper;
using System.Drawing;

namespace Scheduler.API.Models.Document
{
    public class DocumentInfo
    {
        public int Id { get; set; }
        public string? DocumentType { get; set; }
        public int DocumentTypeId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? AccessRoles { get; set; }
        public string? DocumentPath { get; set; }
        public Guid UserId { get; set; }

        public string? RelativeDocumentPath
        {
            get
            {
                if (string.IsNullOrEmpty(DocumentPath)) return null;
                var baseDir = @"C:\\FileStorage\\UserDocument";
                var relativePath = DocumentPath.Substring(baseDir.Length).Replace("\\", "/");
                return $"/UserDocument/{relativePath}";
                
            }
        }
    }
}
