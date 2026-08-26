using Scheduler.API.Models.Staff;

namespace Scheduler.API.Services.Staff
{
    public interface IStaff
    {
        Task<StaffSearchResponse> GetStaffsAsync(StaffSearchRequest request);
        Task<StaffInfo> GetStaffInfoAsync(Guid UserId);
        Task<Guid?> CreateUpdateStaffAsync(SaveStaffInfoViewModel saveStaffInfoViewModel);
        Task<Guid> DeleteStaffAsync(Guid id);
        
        // Keep old sync method for backward compatibility
        Guid DeleteStaff(Guid id);
    }
}
