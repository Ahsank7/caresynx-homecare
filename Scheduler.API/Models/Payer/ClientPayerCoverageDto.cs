namespace Scheduler.API.Models.Payer
{
    public class ClientPayerCoverageDto
    {
        public int Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid PayerId { get; set; }
        public string? PayerLegalName { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsDefaultBillTo { get; set; }
        public string? MemberNumber { get; set; }
        public string? PolicyNumber { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
