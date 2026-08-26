using Scheduler.API.Helper;

namespace Scheduler.API.Models.Lookup
{
    public class LookupDetail
    {
        public int Id { get; set; }
        public string? LookupType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
