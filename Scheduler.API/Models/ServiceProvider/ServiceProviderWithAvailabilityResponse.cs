namespace Scheduler.API.Models.ServiceProvider
{
    public class ServiceProviderWithAvailabilityResponse
    {
        public List<ServiceProviderWithAvailabilityViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
