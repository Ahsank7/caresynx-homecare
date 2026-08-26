using Scheduler.API.Models.Preference;

namespace Scheduler.API.Services.Preference
{
    public interface IPreference
    {
        // Client Preferences
        Task<List<ClientPreferenceInfo>> GetClientPreferencesAsync(Guid clientId);
        Task<Guid> UpsertClientPreferenceAsync(UpsertClientPreferenceRequest request, Guid? userId);
        Task<int> DeleteClientPreferenceAsync(Guid id, Guid? userId);

        // Service Provider Attributes
        Task<List<ServiceProviderAttributeInfo>> GetServiceProviderAttributesAsync(Guid serviceProviderId);
        Task<Guid> UpsertServiceProviderAttributeAsync(UpsertServiceProviderAttributeRequest request, Guid? userId);
        Task<int> DeleteServiceProviderAttributeAsync(Guid id, Guid? userId);

        // Matching
        Task<List<MatchingServiceProviderInfo>> GetMatchingServiceProvidersAsync(Guid clientId, Guid? franchiseId);
    }
}

