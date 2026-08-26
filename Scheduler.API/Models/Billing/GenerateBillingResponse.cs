namespace Scheduler.API.Models.Billing
{
    public class GenerateBillingResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public int GeneratedInvoices { get; set; }
    }
} 