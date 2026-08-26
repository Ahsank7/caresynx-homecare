namespace Scheduler.API.Models.PlanBoard
{
    public class UpdateTaskStatusRequest
    {
        public int TaskId { get; set; }
        public int TaskStatus { get; set; }
        public string? StatusNotes { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
