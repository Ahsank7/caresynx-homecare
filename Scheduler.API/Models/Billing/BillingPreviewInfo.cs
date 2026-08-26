namespace Scheduler.API.Models.Billing
{
    public class BillingPreviewInfo
    {
        public int TaskId { get; set; }
        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public Guid ServiceProviderId { get; set; }
        public string ServiceProviderName { get; set; }
        public DateTime TaskDate { get; set; }
        public decimal BillingAmount { get; set; }
        public bool IsConfirmed { get; set; }
        
        // New fields for expenses
        public string RecordType { get; set; }
        public Guid? ExpenseId { get; set; }
        public decimal? ExpenseAmount { get; set; }
        public string ExpenseType { get; set; }
    }
} 