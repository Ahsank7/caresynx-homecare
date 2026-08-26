namespace Scheduler.API.Models.Authentication
{
    public class AuthResponse
    {
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
