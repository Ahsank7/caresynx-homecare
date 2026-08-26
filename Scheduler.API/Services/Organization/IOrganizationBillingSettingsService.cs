using Scheduler.API.Models.Organization;

namespace Scheduler.API.Services.Organization
{
    public interface IOrganizationBillingSettingsService
    {
        Task<OrganizationBillingSettings> GetBillingSettingsAsync(Guid organizationId);
        Task<bool> SaveBillingSettingsAsync(OrganizationBillingSettingsRequest request);
        Task<List<OrganizationTimeBasedRate>> GetTimeBasedRatesAsync(Guid organizationId);
        Task<OrganizationTimeBasedRate> SaveTimeBasedRateAsync(OrganizationTimeBasedRateRequest request);
        Task<bool> DeleteTimeBasedRateAsync(int id, Guid organizationId);
    }
}
