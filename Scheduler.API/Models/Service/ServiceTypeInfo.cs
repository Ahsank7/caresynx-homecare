namespace Scheduler.API.Models.Service
{
    public class ServiceTypeInfo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
