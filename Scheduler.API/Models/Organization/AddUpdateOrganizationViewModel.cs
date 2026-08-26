namespace Scheduler.API.Models.Organization
{
    public class AddUpdateOrganizationViewModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal DefaultBillingRate { get; set; }
        public decimal DefaultWageRate { get; set; }
        public string? CompleteAddress { get; set; }
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        public string? WebSite { get; set; }
            public int CurrencyId { get; set; }
    public int? CurrencySignId { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TaxPercentage { get; set; }
    public int CalculationTypeId { get; set; }
    public string? LogoPath { get; set; }
    public string? TimeZone { get; set; }
    public int ServiceRateForBilling { get; set; }

    }
}
