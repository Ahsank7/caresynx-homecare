namespace Scheduler.API.Models.LoginHistory
{
    public class InsertLoginHistoryRequest
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string? UserEmail { get; set; }
        public int UserType { get; set; }
        public Guid OrganizationId { get; set; }
        public Guid? FranchiseId { get; set; }
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? BrowserName { get; set; }
        public string? BrowserVersion { get; set; }
        public string? OperatingSystem { get; set; }
        public string? DeviceType { get; set; }
        public string? ScreenResolution { get; set; }
        public string? Timezone { get; set; }
        public string? Language { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string LoginStatus { get; set; } = "Success";
        public string? FailureReason { get; set; }
    }
}
