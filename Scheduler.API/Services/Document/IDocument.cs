using Scheduler.API.Helper;
using Scheduler.API.Models.Document;

namespace Scheduler.API.Services.Document
{
    public interface IDocument
    {
        Task<DocumentInfo> GeDocumentInfoByIdAsync(int Id);
        Task<DocumentInfo> GeDocumentInfoByUserIdAsync(Guid UserId, int documentTypeId);
        Task<int> UploadDocumentAsync(DocumentUploadModel documentUploadModel);
        Task<DocumentSearchResponse> GetUserDocumentsAsync(DocumentSearchRequest request);
        bool DeleteDocument(int Id);
    }
}
