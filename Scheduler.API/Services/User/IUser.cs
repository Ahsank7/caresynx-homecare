using Scheduler.API.Models.User;

namespace Scheduler.API.Services.User
{
    public interface IUser
    {
        Task<UserSearchResponse> GetUsersAsync(UserSearchRequest request);
        Task<UserInfo> GetUserInfoAsync(string login,string password);
        Task<UserInfo> GetUserInfoAsync(Guid userId);
        Task<UserInfo> GetUserInfoByUserNoAsync(string userNo);
        Task<Guid?> CreateUpdateUserAsync(SaveUserInfoViewModel savePatientInfoViewModel);
        Task<Guid> DeleteUserAsync(Guid id, int userStatusAction);
        Task<bool> UploadProfileImageAsync(Guid userId, string profileImagePath);
        
        // Keep old sync methods for backward compatibility
        Guid DeleteUser(Guid id, int userStatusAction);
        Task<bool> UploadProfileImage(Guid userId, string profileImagePath);
    }
}
