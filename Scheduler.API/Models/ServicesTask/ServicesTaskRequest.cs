namespace Scheduler.API.Models.ServicesTask
{
    public class ServicesTaskRequest
    {
        public string? ClientName { get; set; }
        public string? ClientUserNo { get; set; }
        public string? ServiceProviderName { get; set; }
        public string? ServiceProviderUserNo { get; set; }
        public Guid FranchiseId { get; set; }
        public string? TaskStatusIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? TaskId { get; set; }
        public string? SortColumn { get; set; }
        public string? SortType { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
