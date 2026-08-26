namespace Scheduler.API.Models.Preference
{
    public class MatchingServiceProviderInfo
    {
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? SurName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? MobileNo { get; set; }
        public string? PhoneNo { get; set; }
        public int? GenderId { get; set; }
        public Guid? FranchiseId { get; set; }
        public int MatchScore { get; set; }
        public bool MeetsRequiredPreferences { get; set; }
    }
}

