using Scheduler.API.Models.Payer;

namespace Scheduler.API.Services.Payer
{
    public interface IClientPayerService
    {
        Task<IReadOnlyList<PayerDto>> GetPayersByOrganizationAsync(Guid organizationId);
        Task<Guid> SavePayerAsync(PayerDto model);
        Task<IReadOnlyList<ClientPayerCoverageDto>> GetClientCoverageAsync(Guid clientId);
        Task<int> SaveClientCoverageAsync(ClientPayerCoverageDto model);
        Task<ClientBillingPreferenceDto> GetClientBillingPreferenceAsync(Guid clientId);
        Task SaveClientBillingPreferenceAsync(ClientBillingPreferenceDto model);
        Task<IReadOnlyList<ClientPayerServiceFundingDto>> GetFundingRulesAsync(Guid clientId, Guid organizationId);
        Task<int> SaveFundingRuleAsync(ClientPayerServiceFundingDto model);
        Task DeleteFundingRuleAsync(int id, Guid organizationId);

        Task<IReadOnlyList<OrganizationPayerServiceFundingDto>> GetOrganizationFundingRulesAsync(Guid organizationId);
        Task<int> SaveOrganizationFundingRuleAsync(OrganizationPayerServiceFundingDto model);
        Task DeleteOrganizationFundingRuleAsync(int id, Guid organizationId);

        Task<PayerCardInfoDto?> GetPayerCardAsync(Guid organizationId, Guid payerId);
        Task<Guid> UpsertPayerCardAsync(UpsertPayerCardViewModel model);
    }
}
