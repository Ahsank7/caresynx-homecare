namespace Scheduler.API.Models.PlanBoard
{
    public class AssignServiceProviderRequest
    {
        public int TaskId { get; set; }
        public Guid ServiceProviderId { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}

