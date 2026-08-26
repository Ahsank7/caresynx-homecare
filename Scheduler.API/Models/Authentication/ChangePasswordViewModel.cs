namespace Scheduler.API.Models.Authentication
{
    public class ChangePasswordViewModel
    {
        public Guid UserId { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
} 