namespace Scheduler.API.Models.Notification
{
    public class CreateNotificationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Error, Activity
        public int Priority { get; set; } = 0; // 0=Normal, 1=High, 2=Urgent
        public Guid? OrganizationId { get; set; }
        public Guid? FranchiseId { get; set; }
        public int? TargetRoleId { get; set; }
        public Guid? TargetUserId { get; set; }
        public string? ActivityType { get; set; }
        public string? ActivityEntity { get; set; }
        public Guid? ActivityEntityId { get; set; }
        public string? ActionUrl { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}

