using Scheduler.API.Models.Leave;

namespace Scheduler.API.Services.Leave
{
    public interface ILeave
    {
        Task<UserLeaveSearchResponse> GetUserLeavesAsync(UserLeaveSearchRequest request);
        Task<UserLeaveInfo> GetUserLeaveInfoAsync(Guid UserLeaveID);
        Task<Guid?> CreateUpdateUserLeaveAsync(SaveUserLeaveInfoViewModel saveUserLeaveInfoViewModel);
        Guid? DeleteUserLeave(Guid id);
    }
}
