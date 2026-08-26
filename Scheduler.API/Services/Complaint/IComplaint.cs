using Scheduler.API.Models.Complaint;

namespace Scheduler.API.Services.Complaint
{
    public interface IComplaint
    {
        Task<List<ComplaintInfo>> GetComplaintsAsync(GetComplaintsRequest request);
        Task<ComplaintInfo> GetComplaintByIdAsync(Guid complaintId);
        Task<ComplaintInfo> CreateComplaintAsync(Guid complainedAgainstId, int complainedAgainstType, CreateComplaintRequest request, Guid? createdBy);
        Task<ComplaintInfo> UpdateComplaintAsync(UpdateComplaintRequest request, Guid? updatedBy);
        Task<int> DeleteComplaintAsync(Guid complaintId, Guid? updatedBy);
    }
}

