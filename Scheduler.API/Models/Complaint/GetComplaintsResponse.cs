namespace Scheduler.API.Models.Complaint
{
    public class GetComplaintsResponse
    {
        public List<ComplaintInfo> Complaints { get; set; } = new List<ComplaintInfo>();
        public int TotalCount { get; set; }
    }
}

