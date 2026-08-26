namespace Scheduler.API.Models.Billing
{
    public class BillingInfoRequest
    {
        public Guid? FranchiseId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? Date { get; set; }
        public string? UserNo { get; set; }
        public string? TransactionId { get; set; }
        public string? SortColumn { get; set; }
        public string? SortType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
