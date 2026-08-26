using Scheduler.API.Models.PlanBoard;
using Scheduler.API.Models.ServicesTask;

namespace Scheduler.API.Services.PlanBoard
{
    public interface IPlanBoard
    {
        Task<ServicesTaskResponse> GetPlanBoardTasksAsync(ServicesTaskRequest request);
        Task<bool> UpdateTaskNotesAsync(UpdateNotesRequest request);
        Task<bool> UpdateTaskStatusAsync(UpdateTaskStatusRequest request);
        Task<bool> AssignServiceProviderToTaskAsync(AssignServiceProviderRequest request);
        Task<bool> UnassignServiceProviderFromTaskAsync(UnassignServiceProviderRequest request);
        Task<bool> UpdateTaskAttendanceAsync(UpdateTaskAttendanceRequest updateAttendace);
        Task<bool> AddAttendanceAsync(AddAttendanceRequest addAttendance);
        Task<ServicesTaskDetail> GetServicesTaskInfo(string taskId, Guid franchiseId);

    }
}
