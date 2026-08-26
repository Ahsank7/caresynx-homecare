namespace Scheduler.API.Models.ServiceProvider
{
    public class GetAvailableServiceProviderSearchResponse
    {
        public List<GetAvailableServiceProviderSearchViewModel>? Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
