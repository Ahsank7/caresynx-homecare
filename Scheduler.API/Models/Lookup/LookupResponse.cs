using Scheduler.API.Helper;
using Scheduler.API.Models.ServicesTask;

namespace Scheduler.API.Models.Lookup
{
    public class LookupResponse
    {
        public string? LookupType { get; set; }
        public List<LookupDetail>? Result { get; set; }
        public int TotalRecords { get; set; }
    }
}
