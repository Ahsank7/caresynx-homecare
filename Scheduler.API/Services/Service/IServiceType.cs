using Scheduler.API.Models.Service;

namespace Scheduler.API.Services.Service
{
    public interface IServiceType
    {
        Task<GetServiceTypeResponse> GetServiceTypesAsync(Guid OrganizationId);
        Task<ServiceTypeInfo> CreateServiceTypeAsync(ServiceTypeInfo model);
        Task<ServiceTypeInfo> UpdateServiceTypeAsync(ServiceTypeInfo model);
        Task<bool> DeleteServiceTypeAsync(int id);
    }
}
