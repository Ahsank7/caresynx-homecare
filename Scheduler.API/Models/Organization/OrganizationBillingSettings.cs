namespace Scheduler.API.Models.Organization
{
    public class OrganizationBillingSettings
    {
        public Guid OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public int ServiceRateForBilling { get; set; } // 1 = Default, 2 = Service-Specific, 3 = Time-Based
        public decimal DefaultBillingRate { get; set; }
        public decimal DefaultWageRate { get; set; }
        public List<OrganizationTimeBasedRate> TimeBasedRates { get; set; } = new List<OrganizationTimeBasedRate>();
    }
}
