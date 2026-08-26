namespace Scheduler.API.Models.Scheduler
{
    public class GetClientTasksResponse
    {
        public int TaskId { get; set; }
        public int TaskStatusId { get; set; }
        public String TaskStatus { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string ClientFullName { get; set; }
        public string ServiceProviderFullName { get; set; }
        public Guid ClientId { get; set; }
        public Guid ServiceProviderId { get; set; }
        public int ScheduleId { get; set; }
        public string? ServiceType { get; set; }
        public string? ServiceName { get; set; }
    }
}
