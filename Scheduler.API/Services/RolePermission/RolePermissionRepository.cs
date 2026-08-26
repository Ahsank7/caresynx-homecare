using Dapper;
using Scheduler.API.Models.RolePermission;
using Scheduler.API.Common;
using System.Data;

namespace Scheduler.API.Services.RolePermission
{
    public class RolePermissionRepository : IRolePermission
    {
        private readonly IDapperRepository _dapperRepository;

        public RolePermissionRepository(IDapperRepository dapperRepository)
        {
            _dapperRepository = dapperRepository;
        }

        public async Task<List<MenuPermission>> GetUserMenuPermissionsAsync(Guid userId, Guid? organizationId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pUserId", userId);
            parameters.Add("@pOrganizationId", organizationId);

            var result = await _dapperRepository.QueryAsync<MenuPermission>(
                "GetUserMenuPermissions",
                parameters,
                CommandType.StoredProcedure
            );

            return result?.ToList() ?? new List<MenuPermission>();
        }

        public async Task<List<MenuPermission>> GetAllMenusAsync(Guid? organizationId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pOrganizationId", organizationId);

            var result = await _dapperRepository.QueryAsync<MenuPermission>(
                "GetAllMenus",
                parameters,
                CommandType.StoredProcedure
            );

            return result?.ToList() ?? new List<MenuPermission>();
        }

        public async Task<List<MenuInfo>> GetAllMenusForAdminAsync(Guid organizationId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pOrganizationId", organizationId);

            var result = await _dapperRepository.QueryAsync<MenuInfo>(
                "GetAllMenusForAdmin",
                parameters,
                CommandType.StoredProcedure
            );

            return result?.ToList() ?? new List<MenuInfo>();
        }

        public async Task<bool> UpdateMenuStatusAsync(Guid menuId, bool isActive)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pMenuId", menuId);
            parameters.Add("@pIsActive", isActive);

            var result = await _dapperRepository.ExecuteAsync(
                "UpdateMenuStatus",
                parameters,
                CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<List<MenuPermission>> GetRolePermissionsAsync(int roleId, Guid? organizationId = null)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pRoleId", roleId);
            parameters.Add("@pOrganizationId", organizationId);

            var result = await _dapperRepository.QueryAsync<MenuPermission>(
                "GetRolePermissions",
                parameters,
                CommandType.StoredProcedure
            );

            return result?.ToList() ?? new List<MenuPermission>();
        }

        public async Task<bool> SaveRolePermissionsAsync(SaveRolePermissionsRequest request)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@pRoleId", request.RoleId);
            parameters.Add("@pOrganizationId", request.OrganizationId);
            parameters.Add("@pPermissions", System.Text.Json.JsonSerializer.Serialize(request.Permissions));
            parameters.Add("@pCreatedBy", Guid.Empty); // TODO: Get from current user context

            // Debug logging
            Console.WriteLine($"Saving role permissions - RoleId: {request.RoleId}, OrganizationId: {request.OrganizationId}");
            Console.WriteLine($"Permissions JSON: {System.Text.Json.JsonSerializer.Serialize(request.Permissions)}");

            var result = await _dapperRepository.ExecuteAsync(
                "SaveRolePermissions",
                parameters,
                CommandType.StoredProcedure
            );

            Console.WriteLine($"Save result: {result}");
            return result > 0;
        }
    }
} 