using Scheduler.API.Models.Role;

namespace Scheduler.API.Services.Role
{
    public interface IRole
    {
        Task<List<RoleInfo>> GetAvailableRolesAsync(Guid? organizationId = null);
        Task<List<RoleInfo>> GetAvailableRolesForUserAsync(Guid? organizationId, Guid? currentUserId);
        Task<RoleInfo> GetRoleByIdAsync(int roleId);
        Task<bool> CreateRoleAsync(RoleInfo role);
        Task<bool> UpdateRoleAsync(RoleInfo role);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<bool> AssignRoleToUserAsync(Guid userId, int roleId, Guid? createdBy = null);
    }
}
