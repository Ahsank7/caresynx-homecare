namespace Scheduler.API.Models.Client
{
    public class ClientSearchResponse
    {
        public List<SearchClientViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
