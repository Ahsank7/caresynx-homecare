namespace Scheduler.API.Models.ServiceProvider
{
    public class AvailableServiceProviderSearchRequest
    {
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public Guid FranchiseId { get; set; }
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public Guid OrganizationId { get; set; }
    }
}
