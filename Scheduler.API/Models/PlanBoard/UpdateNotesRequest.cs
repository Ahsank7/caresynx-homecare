namespace Scheduler.API.Models.PlanBoard
{
    public class UpdateNotesRequest
    {
        public string TaskId { get; set; }
        public string Notes { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
