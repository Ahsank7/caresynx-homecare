using Scheduler.API.Models.Franchise;

namespace Scheduler.API.Services.Franchise
{
    public interface IFranchise
    {
        Task<FranchiseInfo> GetFranchiseInfoByIdAsync(Guid franchiseId);
        Task<List<FranchiseInfo>> GetFranchisesByOrganizationIdAsync(Guid organisationId);
        Task<List<FranchiseInfo>> GetFranchisesByOrganizationIdAsync(Guid organisationId,Guid userId);
        Task<Guid?> CreateOrUpdateFranchiseAsync(AddOrUpdateFranchiseViewModel saveFranchiseViewModel);
        Task<Guid?> CreateFranchiseAdminUserAsync(CreateFranchiseAdminUserViewModel model);
        Guid DeleteFranchise(Guid id);
        Task<FranchiseDashboardResponse> GetFranchiseDashboardDataAsync(Guid franchiseId, DateTime startDate, DateTime endDate);
        
        // User Franchise Assignment methods
        Task<List<UserFranchiseAssignmentViewModel>> GetUserFranchiseAssignmentsAsync(Guid userId, Guid organizationId);
        Task<bool> AssignUserToFranchiseAsync(AssignUserFranchiseRequest request);
        Task<bool> RemoveUserFromFranchiseAsync(Guid userId, Guid franchiseId);
    }
}
