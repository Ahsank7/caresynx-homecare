namespace Scheduler.API.Models.Franchise
{
    public class CreateFranchiseAdminUserViewModel
    {
        public Guid FranchiseId { get; set; }
        public string FranchiseName { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;
        public Guid OrganizationId { get; set; }
    }
}

