namespace Scheduler.API.Models.Franchise
{
    public class FranchiseInfo
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string logo { get; set; }
        public Guid OrganizationId { get; set; }
        public bool IsActive { get; set; }
    }
}
