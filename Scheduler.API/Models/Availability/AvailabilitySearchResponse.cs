namespace Scheduler.API.Models.Availability
{
    public class AvailabilitySearchResponse
    {
        public List<SearchAvailabilityViewModel> Response { get; set; }
        public int TotalRecords { get; set; }
    }
}
