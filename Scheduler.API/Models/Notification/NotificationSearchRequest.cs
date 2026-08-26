namespace Scheduler.API.Models.Notification
{
    public class NotificationSearchRequest
    {
        public Guid UserId { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? FranchiseId { get; set; }
        public int? RoleId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool UnreadOnly { get; set; } = false;
    }
}

