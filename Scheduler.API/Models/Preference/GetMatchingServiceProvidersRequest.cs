namespace Scheduler.API.Models.Preference
{
    public class GetMatchingServiceProvidersRequest
    {
        public Guid ClientId { get; set; }
        public Guid? FranchiseId { get; set; }
    }
}

