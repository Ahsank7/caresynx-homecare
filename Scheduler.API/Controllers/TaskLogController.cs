using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Common;
using Scheduler.API.Models.TaskLog;
using Scheduler.API.Services.TaskLog;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskLogController : ControllerBase
    {
        private readonly ITaskLog _taskLog;

        public TaskLogController(ITaskLog taskLog)
        {
            _taskLog = taskLog;
        }

        [HttpPost]
        [Route("GetTaskLogs")]
        public async Task<IActionResult> GetTaskLogsAsync(TaskLogRequest request)
        {
            var result = await _taskLog.GetTaskLogsAsync(request);
            return Ok(new Response<TaskLogResponse> { Status = StatusCodes.Status200OK, Message = "Task logs retrieved successfully!", Data = result, IsSuccess = true });
        }

        [HttpPost]
        [Route("InsertTaskLog")]
        public async Task<IActionResult> InsertTaskLogAsync(int taskId, string actionType, string? previousValue = null, string? newValue = null, string? fieldName = null, string? description = null)
        {
            var result = await _taskLog.InsertTaskLogAsync(taskId, actionType, previousValue, newValue, fieldName, description);
            return Ok(new Response<int> { Status = StatusCodes.Status200OK, Message = "Task log inserted successfully!", Data = result, IsSuccess = true });
        }
    }
}
