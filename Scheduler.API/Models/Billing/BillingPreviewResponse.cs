namespace Scheduler.API.Models.Billing
{
    public class BillingPreviewResponse
    {
        public List<BillingPreviewInfo> Response { get; set; } = new List<BillingPreviewInfo>();
        public int TotalRecords { get; set; }
    }
} 