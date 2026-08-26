using Scheduler.API.Models.Scheduler;

namespace Scheduler.API.Services.Scheduler
{
    public interface IScheduler
    {
        Task<ScheduleAppointmentResponse> CreateSceduleAppointmentAsync(ScheduleAppointmentRequest request);
        Task<List<GetClientTasksResponse>> GetClientTasks(GetClientTasksRequest request);
        Task<List<GetServiceProviderTasksResponse>> GetServiceProviderTasks(GetServiceProviderTasksRequest request);


    }
}
