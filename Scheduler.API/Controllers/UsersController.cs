using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.User;
using Scheduler.API.Services.FileStorage;
using Scheduler.API.Services.User;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUser _User;
        private readonly IFileStorageService _fileStorageService;

        public UsersController(IUser User, IFileStorageService fileStorageService, ILogger<UsersController> logger) 
            : base(logger)
        {
            _User = User;
            _fileStorageService = fileStorageService;
        }

        [HttpPost("SaveUpdate")]
        public async Task<IActionResult> SaveUpdateUser(SaveUserInfoViewModel model)
        {
            if (model == null)
                return ValidationError("User data is required");

            return await ExecuteAsync(
                () => _User.CreateUpdateUserAsync(model),
                "User created/Updated successfully!"
            );
        }

        [HttpGet("Details")]
        public async Task<IActionResult> GetUserDetails(Guid UserId)
        {
            if (UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(
                () => _User.GetUserInfoAsync(UserId),
                "User details retrieved successfully!"
            );
        }

        [HttpPost("List")]
        public async Task<IActionResult> GetUserList(UserSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search criteria is required");

            // Get current user ID from JWT for role hierarchy filtering (when fetching staff)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                model.CurrentUserId = currentUserId;
            }

            return await ExecuteAsync(
                () => _User.GetUsersAsync(model),
                "User list retrieved successfully!"
            );
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser(Guid Id, int userStatusAction)
        {
            if (Id == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(
                () => _User.DeleteUserAsync(Id, userStatusAction),
                "User deleted successfully!"
            );
        }
    }
}
