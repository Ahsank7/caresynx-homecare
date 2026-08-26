namespace Scheduler.API.Models.ServiceProvider
{
    public class ServiceProviderSearchRequest
    {
        public Guid? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? MobileNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? JoiningDate { get; set; }
        public int? GenderId { get; set; }
        public int? StatusId { get; set; }
        public int? EthnicityId { get; set; }
        public string SortColumn { get; set; }
        public string SortType { get; set; }
        public Guid FranchiseId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        
        // Preference filtering options
        public Guid? ClientId { get; set; }
        public bool ApplyPreferenceFilter { get; set; } = false;
    }
}
