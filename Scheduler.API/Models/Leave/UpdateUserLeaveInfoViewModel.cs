namespace Scheduler.API.Models.Leave
{
    public class UpdateUserExpenseInfoViewModel
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int TaskId { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
    }
}
