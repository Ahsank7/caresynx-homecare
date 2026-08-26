namespace Scheduler.API.Models.LoginHistory
{
    public class LoginHistoryRequest
    {
        public Guid OrganizationId { get; set; }
        public Guid? UserId { get; set; }
        public int? UserType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? LoginStatus { get; set; }
        public string? IPAddress { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string SortColumn { get; set; } = "LoginTime";
        public string SortDirection { get; set; } = "DESC";
    }
}
