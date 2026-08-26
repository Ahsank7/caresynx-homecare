namespace Scheduler.API.Models.Authentication
{
    public class UserNameCheckResult
    {
        public int UserNameExists { get; set; }
        public int IsUserNameExists { get; set; }
        public Guid UserId { get; set; }
    }
}
