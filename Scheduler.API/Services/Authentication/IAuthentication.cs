using Scheduler.API.Models.Authentication;

namespace Scheduler.API.Services.Authentication
{
    public interface IAuthentication
    {
        Task<AuthResponse> Authenticate(UserLoginRequest request);
        Task<UserAuthenticationInfo> GetAuthenticationInfoAsync(Guid UserId);
        Task<Guid?> UpdateUserAuthenticationInfo(UpdateUserAuthenticationInfoViewModel saveAuthenticationInfoViewModel);
        Task<bool> ChangePassword(ChangePasswordViewModel changePasswordViewModel);
        Task<UserNameCheckResult> UserNameExistsAsync(string userName);
    }
}
