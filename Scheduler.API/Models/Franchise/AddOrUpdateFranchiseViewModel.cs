namespace Scheduler.API.Models.Franchise
{
    public class AddOrUpdateFranchiseViewModel
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string logo { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? UserId { get; set; }
        public bool? IsActive { get; set; }
    }
}
