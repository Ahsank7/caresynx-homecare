namespace Scheduler.API.Models.Scheduler
{
    public class GetClientTasksRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid ClientId { get; set; }
        public string StatusIds { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
