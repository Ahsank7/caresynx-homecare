using Scheduler.API.Models.ServiceProviderContract;

namespace Scheduler.API.Services.ServiceProviderContract
{
    public interface IServiceProviderContract
    {
        Task<ContractInfoResponse> GetServiceProviderContractsAsync(Guid serviceProviderId);
        Task<ContractDetailViewModel> GetServiceProviderContractInfoAsync(int contractId);
        Task<int> UpsertServiceProviderContractAsync(SaveUpdateContractViewModel upsertContractViewModel);
    }
}
