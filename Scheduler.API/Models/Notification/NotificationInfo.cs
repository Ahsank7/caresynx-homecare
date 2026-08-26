namespace Scheduler.API.Models.Notification
{
    public class NotificationInfo
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "Info"; // Info, Success, Warning, Error, Activity
        public int Priority { get; set; } = 0; // 0=Normal, 1=High, 2=Urgent
        public Guid? OrganizationId { get; set; }
        public Guid? FranchiseId { get; set; }
        public int? TargetRoleId { get; set; }
        public Guid? TargetUserId { get; set; }
        public string? ActivityType { get; set; } // Create, Update, Delete, Custom
        public string? ActivityEntity { get; set; } // Client, Staff, Schedule, etc.
        public Guid? ActivityEntityId { get; set; }
        public string? ActionUrl { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime? ReadDate { get; set; }
    }
}

