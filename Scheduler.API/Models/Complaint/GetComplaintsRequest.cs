namespace Scheduler.API.Models.Complaint
{
    public class GetComplaintsRequest
    {
        public Guid? UserId { get; set; }
        public Guid? ComplainantId { get; set; }
        public Guid? ComplainedAgainstId { get; set; }
        public Guid? FranchiseId { get; set; }
        public int? Status { get; set; }
        public int? Category { get; set; }
        public int? Severity { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }
}

