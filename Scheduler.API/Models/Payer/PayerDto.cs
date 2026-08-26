namespace Scheduler.API.Models.Payer
{
    public class PayerDto
    {
        public Guid? Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string LegalName { get; set; } = string.Empty;
        public byte PayerType { get; set; }
        public string? BillingAddressLine1 { get; set; }
        public string? BillingAddressLine2 { get; set; }
        public string? BillingAddressLine3 { get; set; }
        public string? BillingEmail { get; set; }
        public int? DefaultPaymentTermsDays { get; set; }
        public string? TaxId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
