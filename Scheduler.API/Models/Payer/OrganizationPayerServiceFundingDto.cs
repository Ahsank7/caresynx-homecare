namespace Scheduler.API.Models.Payer
{
    public class OrganizationPayerServiceFundingDto
    {
        public int Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid PayerId { get; set; }
        public int? ServiceId { get; set; }
        public decimal FundedPercent { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
