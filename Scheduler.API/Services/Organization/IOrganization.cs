using Scheduler.API.Models.Organization;

namespace Scheduler.API.Services.Organization
{
    public interface IOrganization
    {
        Task<OrganizationInfo> GetOrganisationInfoByIdAsync(Guid OrganisationId);
        Task<List<OrganizationInfo>> GetOrganizationsByUserIdAsync(Guid userId);
        Task<List<OrganizationInfo>> GetAllOrganizationsAsync();
        Task<Guid?> CreateUpdateOrganizationAsync(AddUpdateOrganizationViewModel saveOrganizationViewModel);
        Guid DeleteOrganization(Guid id);
        Task<bool> UpdateOrganizationLogoAsync(Guid organizationId, string logoPath);
        Task<bool> ClearOrganizationLogoAsync(Guid organizationId);
    }
}
