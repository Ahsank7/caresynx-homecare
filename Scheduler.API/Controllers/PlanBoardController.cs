using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.PlanBoard;
using Scheduler.API.Services.PlanBoard;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Scheduler.API.Models.ServicesTask;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlanBoardController : BaseController
    {
        IPlanBoard _planboard;
        public PlanBoardController(IPlanBoard planboard, ILogger<PlanBoardController> logger) : base(logger)
        {
            _planboard = planboard;
        }

        [HttpPost]
        [Route("ServicesTask")]
        public async Task<IActionResult> GetPlanBoardTasksAsync(ServicesTaskRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _planboard.GetPlanBoardTasksAsync(request),
                "Plan board tasks retrieved successfully!"
            );
        }

        [HttpPost]
        [Route("UpdateTaskNotes")]
        public async Task<IActionResult> UpdateTaskNotes(UpdateNotesRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _planboard.UpdateTaskNotesAsync(request),
                "Task Notes Updated successfully!"
            );
        }
        [HttpPost]
        [Route("UpdateTaskAttendance")]
        public async Task<IActionResult> UpdateTaskAttendance(UpdateTaskAttendanceRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _planboard.UpdateTaskAttendanceAsync(request),
                "Task Attendance Updated successfully!"
            );
        }
        [HttpPost]
        [Route("UpdateTaskStatus")]
        public async Task<IActionResult> UpdateTaskStatus(UpdateTaskStatusRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(async () =>
            {
                // Use the TaskStatus value from the request (don't override it)
                return await _planboard.UpdateTaskStatusAsync(request);
            }, "Task Status Updated successfully!");
        }

        [HttpPost]
        [Route("AssignServiceProvider")]
        public async Task<IActionResult> AssignServiceProvider(AssignServiceProviderRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            if (request.ServiceProviderId == Guid.Empty)
                return ValidationError("Valid Service Provider ID is required");

            return await ExecuteAsync(async () =>
            {
                return await _planboard.AssignServiceProviderToTaskAsync(request);
            }, "Service Provider assigned successfully!");
        }

        [HttpPost]
        [Route("UnassignServiceProvider")]
        public async Task<IActionResult> UnassignServiceProvider(UnassignServiceProviderRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            if (request.TaskId <= 0)
                return ValidationError("Valid Task ID is required");

            return await ExecuteAsync(
                () => _planboard.UnassignServiceProviderFromTaskAsync(request),
                "Service provider unassigned successfully!"
            );
        }
        [HttpPost]
        [Route("AddTaskAttendance")]
        public async Task<IActionResult> AddTaskAttendance(AddAttendanceRequest request)
        {
            if (request == null)
                return ValidationError("Request data is required");

            return await ExecuteAsync(
                () => _planboard.AddAttendanceAsync(request),
                "Attendance Added successfully!"
            );
        }

        [HttpGet]
        [Route("ServicesTaskInfo")]
        public async Task<IActionResult> GetServicesTaskInfo(string taskId, Guid franchiseId)
        {
            if (string.IsNullOrEmpty(taskId))
                return ValidationError("Task ID is required");

            if (franchiseId == Guid.Empty)
                return ValidationError("Valid Franchise ID is required");

            return await ExecuteAsync(
                () => _planboard.GetServicesTaskInfo(taskId, franchiseId),
                "Services Task details retrieved successfully!"
            );
        }
    }
}
