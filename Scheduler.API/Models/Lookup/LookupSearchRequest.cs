using Scheduler.API.Helper;

namespace Scheduler.API.Models.Lookup
{
    public class LookupSearchRequest
    {
        public string? LookupType { get; set; }
        public string? Name { get; set; }
        public bool? IsActive { get; set; }
        public string? SortColumn { get; set; }
        public string? SortType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
