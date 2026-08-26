namespace Scheduler.API.Models.Billing
{
    public class BillingInfo
    {
        public int Id { get; set; }
        public string? Details { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountAfterTax { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal AmountAfterDiscount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; }
        public Guid ClientId { get; set; }
        public Guid? TransactionId { get; set; }
        public string? ServiceProviderName { get; set; }
        public string? ClientName { get; set; }
        public string? UserNo { get; set; }
        public string? BillingPaymentsId { get; set; }
        public byte? BillToType { get; set; }
        public Guid? BillToPayerId { get; set; }
        public Guid? BillToUserContactId { get; set; }
        public string? BillToDisplayName { get; set; }
        public string? DebtorEmail { get; set; }
        /// <summary>Resolved from debtor (client, payer, or guarantor) for display/PDF.</summary>
        public string? BillToAddress { get; set; }
        public string? BillToPhone { get; set; }
    }
}
