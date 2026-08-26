namespace Scheduler.API.Models.PlanBoard
{
    public class UnassignServiceProviderRequest
    {
        public int TaskId { get; set; }
        public string? Notes { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
