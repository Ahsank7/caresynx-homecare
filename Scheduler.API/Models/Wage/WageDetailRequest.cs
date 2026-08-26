namespace Scheduler.API.Models.Wage
{
    public class WageDetailRequest
    {
        public int WageId { get; set; }
        public string? SortColumn { get; set; }
        public string? SortType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
