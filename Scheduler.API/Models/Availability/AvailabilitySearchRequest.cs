namespace Scheduler.API.Models.Availability
{
    public class AvailabilitySearchRequest
    {
        public Guid? UserId { get; set; }
        public string SortColumn { get; set; }
        public string SortType { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
