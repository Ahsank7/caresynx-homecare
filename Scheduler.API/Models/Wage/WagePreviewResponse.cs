namespace Scheduler.API.Models.Wage
{
    public class WagePreviewResponse
    {
        public List<WagePreviewInfo> Response { get; set; } = new List<WagePreviewInfo>();
        public int TotalRecords { get; set; }
    }
} 