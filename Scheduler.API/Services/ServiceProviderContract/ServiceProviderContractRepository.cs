using Scheduler.API.Models.ServiceProviderContract;

namespace Scheduler.API.Services.ServiceProviderContract
{
    public class ServiceProviderContractRepository : IServiceProviderContract
    {
        IDapperRepository _dapperRepository = null;
        public ServiceProviderContractRepository(IDapperRepository DapperRepository)
        {
            _dapperRepository = DapperRepository;
        }

        public Task<ContractDetailViewModel> GetServiceProviderContractInfoAsync(int contractId)
        {
            throw new NotImplementedException();
        }

        public Task<ContractInfoResponse> GetServiceProviderContractsAsync(Guid serviceProviderId)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpsertServiceProviderContractAsync(SaveUpdateContractViewModel upsertContractViewModel)
        {
            throw new NotImplementedException();
        }
    }
}
