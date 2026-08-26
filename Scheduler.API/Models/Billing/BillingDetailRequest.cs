namespace Scheduler.API.Models.Billing
{
    public class BillingDetailRequest
    {
        public int BillingId { get; set; }
        public string? SortColumn { get; set; }
        public string? SortType { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
