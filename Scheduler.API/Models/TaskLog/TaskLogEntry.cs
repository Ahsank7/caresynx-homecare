namespace Scheduler.API.Models.TaskLog
{
    public class TaskLogEntry
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public string ActionType { get; set; }
        public string? PreviousValue { get; set; }
        public string? NewValue { get; set; }
        public string? FieldName { get; set; }
        public string? Description { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string UserName { get; set; }
        public string UserNo { get; set; }
    }
}
