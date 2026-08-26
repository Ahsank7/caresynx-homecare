using Scheduler.API.Models.LoginHistory;

namespace Scheduler.API.Services.LoginHistory
{
    public interface ILoginHistory
    {
        Task<LoginHistoryResponse> GetLoginHistoryAsync(LoginHistoryRequest request);
        Task<int> InsertLoginHistoryAsync(InsertLoginHistoryRequest request);
        Task<bool> UpdateLogoutTimeAsync(Guid userId, DateTime? loginTime = null);
    }
}
