namespace Scheduler.API.Models.Scheduler
{
    public class GetServiceProviderTasksRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ServiceProviderId { get; set; }
        public string StatusIds { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
