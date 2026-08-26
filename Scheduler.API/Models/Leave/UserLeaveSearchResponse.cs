namespace Scheduler.API.Models.Leave
{
    public class UserLeaveSearchResponse
    {
        public List<SearchUserLeaveViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
