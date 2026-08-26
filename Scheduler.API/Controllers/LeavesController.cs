using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.Leave;
using Scheduler.API.Services.Leave;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [ApiController]
    [Authorize]
    public class LeavesController : BaseController
    {
        ILeave _Leave;
        public LeavesController(ILeave Leave, ILogger<LeavesController> logger) : base(logger)
        {
            _Leave = Leave;
        }

        [HttpPost]
        [Route("SaveUpdateUserLeave")]
        public async Task<IActionResult> SaveUpdateUserLeave(SaveUserLeaveInfoViewModel model)
        {
            if (model == null)
                return ValidationError("Leave data is required");

            return await ExecuteAsync(
                () => _Leave.CreateUpdateUserLeaveAsync(model),
                "User Leave created/Updated successfully!"
            );
        }

        [HttpGet]
        [Route("GetUserLeaveDetails")]
        public async Task<IActionResult> GetUserLeaveDetails(Guid userLeaveId)
        {
            if (userLeaveId == Guid.Empty)
                return ValidationError("Valid leave ID is required");

            return await ExecuteAsync(
                () => _Leave.GetUserLeaveInfoAsync(userLeaveId),
                "User Leave details retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("GetUserLeaveList")]
        public async Task<IActionResult> GetUserLeaveList(UserLeaveSearchRequest model)
        {
            if (model == null)
                return ValidationError("Search request data is required");

            return await ExecuteAsync(
                () => _Leave.GetUserLeavesAsync(model),
                "User Leave list retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("Delete")]
        public IActionResult DeleteUserLeave(Guid userLeaveId)
        {
            if (userLeaveId == Guid.Empty)
                return ValidationError("Valid leave ID is required");

            return Execute(() =>
            {
                var result = _Leave.DeleteUserLeave(userLeaveId);
                return result.Value;
            }, "User Leave deleted successfully!");
        }
    }
}
