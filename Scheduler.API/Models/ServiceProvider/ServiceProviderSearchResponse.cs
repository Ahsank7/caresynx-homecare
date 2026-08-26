namespace Scheduler.API.Models.ServiceProvider
{
    public class ServiceProviderSearchResponse
    {
        public List<SearchServiceProviderViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
