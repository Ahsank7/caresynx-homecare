namespace Scheduler.API.Models.TaskLog
{
    public class TaskLogResponse
    {
        public List<TaskLogEntry> Logs { get; set; } = new List<TaskLogEntry>();
        public int TotalRecords { get; set; }
    }
}
