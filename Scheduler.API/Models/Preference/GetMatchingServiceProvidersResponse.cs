namespace Scheduler.API.Models.Preference
{
    public class GetMatchingServiceProvidersResponse
    {
        public List<MatchingServiceProviderInfo> ServiceProviders { get; set; } = new List<MatchingServiceProviderInfo>();
    }
}

