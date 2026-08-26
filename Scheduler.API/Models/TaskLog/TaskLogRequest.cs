namespace Scheduler.API.Models.TaskLog
{
    public class TaskLogRequest
    {
        public int TaskId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
