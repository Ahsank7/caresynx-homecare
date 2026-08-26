namespace Scheduler.API.Models.Billing
{
    public class BillingPreviewRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 15;
        public string? SortColumn { get; set; }
        public string? SortType { get; set; }
    }
} 