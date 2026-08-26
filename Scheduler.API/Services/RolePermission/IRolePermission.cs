using Scheduler.API.Models.RolePermission;

namespace Scheduler.API.Services.RolePermission
{
    public interface IRolePermission
    {
        Task<List<MenuPermission>> GetUserMenuPermissionsAsync(Guid userId, Guid? organizationId = null);
        Task<List<MenuPermission>> GetAllMenusAsync(Guid? organizationId);
        Task<List<MenuInfo>> GetAllMenusForAdminAsync(Guid organizationId);
        Task<bool> UpdateMenuStatusAsync(Guid menuId, bool isActive);
        Task<List<MenuPermission>> GetRolePermissionsAsync(int roleId, Guid? organizationId = null);
        Task<bool> SaveRolePermissionsAsync(SaveRolePermissionsRequest request);
    }
} 