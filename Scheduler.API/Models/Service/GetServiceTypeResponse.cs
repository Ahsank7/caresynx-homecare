namespace Scheduler.API.Models.Service
{
    public class GetServiceTypeResponse
    {
        public List<ServiceTypeInfo> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
