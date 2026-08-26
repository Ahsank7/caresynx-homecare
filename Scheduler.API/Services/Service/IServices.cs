using Scheduler.API.Models.Service;

namespace Scheduler.API.Services.Service
{
    public interface IServices
    {
        Task<GetServicesResponse> GetServiceListAsync(int ServiceTypeId);
        Task<ServiceInfo> CreateServiceAsync(ServiceInfo model);
        Task<ServiceInfo> UpdateServiceAsync(ServiceInfo model);
        Task<bool> DeleteServiceAsync(int id);
    }
}
