using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Staff;
using Scheduler.API.Services.Staff;
using Microsoft.Extensions.Logging;

namespace Scheduler.API.Controllers
{
    public class StaffController : BaseController
    {
        private readonly IStaff _Staff;

        public StaffController(IStaff Staff, ILogger<StaffController> logger) 
            : base(logger)
        {
            _Staff = Staff;
        }

        [HttpPost("SaveUpdate")]
        public async Task<IActionResult> SaveUpdateStaff(SaveStaffInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Staff data is required");

            return await ExecuteAsync(
                () => _Staff.CreateUpdateStaffAsync(model),
                "Staff created/Updated successfully!"
            );
        }

        [HttpGet("GetStaffDetails")]
        public async Task<IActionResult> GetStaffDetails(Guid UserId)
        {
            if (UserId == Guid.Empty)
                return ValidationError("Valid User ID is required");

            return await ExecuteAsync(
                () => _Staff.GetStaffInfoAsync(UserId),
                "Staff details retrieved successfully!"
            );
        }

        [HttpPost("GetStaffList")]
        public async Task<IActionResult> GetStaffList(StaffSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search criteria is required");

            // Get current user ID from JWT for role hierarchy filtering
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out Guid currentUserId))
            {
                model.CurrentUserId = currentUserId;
            }

            return await ExecuteAsync(
                () => _Staff.GetStaffsAsync(model),
                "Staff list retrieved successfully!"
            );
        }

        [HttpDelete("DeleteStaff")]
        public async Task<IActionResult> DeleteStaff(Guid Id)
        {
            if (Id == Guid.Empty)
                return ValidationError("Valid Staff ID is required");

            return await ExecuteAsync(
                () => _Staff.DeleteStaffAsync(Id),
                "Staff deleted successfully!"
            );
        }
    }
}
