using Scheduler.API.Models.TaskLog;

namespace Scheduler.API.Services.TaskLog
{
    public interface ITaskLog
    {
        Task<TaskLogResponse> GetTaskLogsAsync(TaskLogRequest request);
        Task<int> InsertTaskLogAsync(int taskId, string actionType, string? previousValue = null, string? newValue = null, string? fieldName = null, string? description = null, Guid? createdBy = null);
    }
}
