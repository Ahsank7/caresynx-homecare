namespace Scheduler.API.Models.Service
{
    public class GetServicesResponse
    {
        public List<ServiceInfo> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
