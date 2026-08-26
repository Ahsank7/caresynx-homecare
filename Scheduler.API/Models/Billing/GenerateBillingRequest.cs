namespace Scheduler.API.Models.Billing
{
    public class GenerateBillingRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? OrganizationId { get; set; }
    }
} 