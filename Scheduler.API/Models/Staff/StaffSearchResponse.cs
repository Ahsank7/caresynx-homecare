namespace Scheduler.API.Models.Staff
{
    public class StaffSearchResponse
    {
        public List<SearchStaffViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
