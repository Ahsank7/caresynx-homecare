namespace Scheduler.API.Models.Leave
{
    public class SearchUserLeaveViewModel
    {
        public Guid CreatedBy { get; set; }
        public Guid UserId { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
        public string? Type { get; set; }
        public string? Status { get; set; }
        public Guid? Id { get; set; }
        public bool IsActive { get; set; }
    }
}
