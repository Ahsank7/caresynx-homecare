namespace Scheduler.API.Models.Leave
{
    public class UserLeaveSearchRequest
    {
        public Guid? UserId { get; set; }
        public string? Date { get; set; }
        public int? TypeId { get; set; }
        public int? StatusId { get; set; }
        public string SortColumn { get; set; }
        public string SortType { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
