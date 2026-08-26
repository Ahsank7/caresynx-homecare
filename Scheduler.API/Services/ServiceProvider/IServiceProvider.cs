using Scheduler.API.Models.ServiceProvider;

namespace Scheduler.API.Services.ServiceProvider
{
    public interface IServiceProvider
    {
        Task<ServiceProviderSearchResponse> GetServiceProvidersAsync(ServiceProviderSearchRequest request);
        Task<ServiceProviderInfo> GetServiceProviderInfoAsync(Guid UserId);
        Task<Guid?> CreateUpdateServiceProviderAsync(SaveServiceProviderInfoViewModel saveServiceProviderInfoViewModel);
        Guid DeleteServiceProvider(Guid id);
        Task<GetAvailableServiceProviderSearchResponse> GetAvailableServiceProvidersAsync(AvailableServiceProviderSearchRequest request);
        Task<ServiceProviderWithAvailabilityResponse> GetServiceProvidersWithAvailabilityAsync(ServiceProviderWithAvailabilityRequest request);

        Task<ContractInfo> GetContractInfoAsync(Guid UserId);
        Task<Guid?> UpsertContractInfo(UpsertContractInfoViewModel upsertContractInfoViewModel);
    }
}
