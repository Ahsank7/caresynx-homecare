namespace Scheduler.API.Models.LoginHistory
{
    public class LoginHistoryResponse
    {
        public List<LoginHistoryEntry> Entries { get; set; } = new List<LoginHistoryEntry>();
        public int TotalRecords { get; set; }
    }
}
