namespace Scheduler.API.Models.ServiceProvider
{
    public class ServiceProviderWithAvailabilityRequest
    {
        public Guid FranchiseId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string SearchText { get; set; }
    }
}
