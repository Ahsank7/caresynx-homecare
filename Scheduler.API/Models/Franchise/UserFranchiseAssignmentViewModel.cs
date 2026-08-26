namespace Scheduler.API.Models.Franchise
{
    public class UserFranchiseAssignmentViewModel
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FranchiseId { get; set; }
        public string FranchiseName { get; set; }
        public bool IsActive { get; set; }
    }

    public class AssignUserFranchiseRequest
    {
        public Guid UserId { get; set; }
        public Guid FranchiseId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

