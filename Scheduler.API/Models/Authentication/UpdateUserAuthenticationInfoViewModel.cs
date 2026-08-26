using FluentValidation;

namespace Scheduler.API.Models.Authentication
{
    public class UpdateUserAuthenticationInfoViewModel
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public int? RoleId { get; set; }
    }
}
